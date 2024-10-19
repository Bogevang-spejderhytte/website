using Cofoundry.Domain;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace Bogevang.Common.Utility
{
  public static class VueEditorExtensions
  {
    public static async Task<IHtmlContent> VueEditorRowFor<TModel, TResult>(
      this IHtmlHelper<TModel> helper,
      Expression<Func<TModel, TResult>> expression,
      bool alwaysReadonly = false,
      string cssClass = null,
      bool showInactiveItems = false)
    {
      string html = $@"<div class=""row mb-3"">";

      string innerHtml = await helper.VueEditorColFor_string(
        expression, 
        alwaysReadonly: alwaysReadonly, 
        cssClass: cssClass,
        showInactiveItems: showInactiveItems);

      html += innerHtml + @"
</div>";

      return new HtmlString(html);
    }


    public static async Task<IHtmlContent> VueEditorColFor<TModel, TResult>(
      this IHtmlHelper<TModel> helper,
      Expression<Func<TModel, TResult>> expression,
      bool alwaysReadonly = false,
      string cssClass = null,
      bool showInactiveItems = false)
    {
      string html = await helper.VueEditorColFor_string(
        expression, 
        alwaysReadonly: alwaysReadonly, 
        cssClass: cssClass,
        showInactiveItems: showInactiveItems);
      return new HtmlString(html);
    }


    public static async Task<string> VueEditorColFor_string<TModel, TResult>(
      this IHtmlHelper<TModel> helper,
      Expression<Func<TModel, TResult>> expression,
      bool alwaysReadonly = false,
      string cssClass = null,
      bool showInactiveItems = false)
    {
      IModelExpressionProvider expressionProvider = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IModelExpressionProvider>();
      ModelExpression expr = expressionProvider.CreateModelExpression(helper.ViewData, expression);

      string propName = expr.Metadata.PropertyName;
      propName = Char.ToLowerInvariant(propName[0]) + propName.Substring(1);

      string description = expr.Metadata.Description;
      string describedBy = "";
      string descriptionHtml = "";
      if (!string.IsNullOrEmpty(description))
      {
        describedBy = $@" aria-describedby=""{propName}_descr""";
        descriptionHtml = $@"
<div id=""{propName}_descr"" class=""form-text"">
{WebUtility.HtmlEncode(description)}
</div>";
      }

      string html = $@"<div class=""col"">";

      string editor = await helper.VueEditorFor_string(
        expression, 
        addLabel: true, 
        alwaysReadonly: alwaysReadonly,
        describedBy: describedBy,
        cssClass: cssClass,
        showInactiveItems: showInactiveItems);

      html += editor;
      html += descriptionHtml;

      html += $@"
  <div id=""{propName}_feedback"" class=""invalid-feedback""></div>
</div>";

      return html;
    }


    public static async Task<IHtmlContent> VueEditorFor<TModel, TResult>(
      this IHtmlHelper<TModel> helper,
      Expression<Func<TModel, TResult>> expression,
      bool alwaysReadonly = false,
      string cssClass = null,
      bool showInactiveItems = false)
    {
      string html = await helper.VueEditorFor_string(
        expression, 
        addLabel: false, 
        alwaysReadonly: alwaysReadonly, 
        cssClass: cssClass,
        showInactiveItems: showInactiveItems);
      return new HtmlString(html);
    }


    public static async Task<string> VueEditorFor_string<TModel,TResult>(
      this IHtmlHelper<TModel> helper,
      Expression<Func<TModel, TResult>> expression,
      bool addLabel,
      bool alwaysReadonly = false,
      string describedBy = null,
      string cssClass = null,
      bool showInactiveItems = false)
    {
      IModelExpressionProvider expressionProvider = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IModelExpressionProvider>();
      ModelExpression expr = expressionProvider.CreateModelExpression(helper.ViewData, expression);

      string propName = expr.Metadata.PropertyName;
      propName = Char.ToLowerInvariant(propName[0]) + propName.Substring(1);

      string displayName = expr.Metadata.DisplayName ?? "";
      if (displayName.StartsWith("html:"))
        displayName = displayName.Substring(5);
      else
        displayName = WebUtility.HtmlEncode(displayName);

      Type modelType = expr.Metadata.ModelType;
      modelType = Nullable.GetUnderlyingType(modelType) ?? modelType;

      bool isBool = modelType == typeof(bool);
      bool isEnum = modelType.IsEnum;
      bool isDate = modelType.IsAssignableFrom(typeof(DateTime));
      bool isMultiLine = false;
      bool isHtml = false;
      bool isNumber = modelType == typeof(int) || modelType == typeof(decimal) || modelType == typeof(double);

      string editableClass = alwaysReadonly ? "" : " editable";

      MultiLineTextAttribute matt = null;

      Type enumType = null;
      List<CustomEntityRenderSummary> customEntities = null;
      SelectListItem[] enumCollectionItems = null;

      MemberInfo[] memberInfo = expr.Metadata.ContainerType.GetMember(expr.Metadata.PropertyName);

      RadioListAttribute radioListAtt = (RadioListAttribute)memberInfo[0].GetCustomAttributes(typeof(RadioListAttribute), false).FirstOrDefault();

      bool implementICollection = modelType.IsGenericType && modelType.GetGenericTypeDefinition() == typeof(ICollection<>);

      if (modelType == typeof(int))
      {
        CustomEntityAttribute att = (CustomEntityAttribute)memberInfo[0].GetCustomAttributes(typeof(CustomEntityAttribute), false).FirstOrDefault();
        if (att != null)
        {
          customEntities = (await helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IAdvancedContentRepository>()
            .CustomEntities()
            .GetByDefinitionCode(att.CustomEntityDefinitionCode)
            .AsRenderSummaries()
            .ExecuteAsync())
            .Where(e => showInactiveItems || (e.Model is IActiveState a && a.IsActive))
            .OrderBy(e => e.Ordering)
            .ToList();
        }
      }
      else if (implementICollection)
      {
        CheckboxListAttribute att = (CheckboxListAttribute)memberInfo[0].GetCustomAttributes(typeof(CheckboxListAttribute), false).FirstOrDefault();
        if (att != null)
        {
          if (att.OptionSource.IsEnum)
          {
            enumType = Nullable.GetUnderlyingType(att.OptionSource) ?? att.OptionSource;
          }
        }
      }
      else if (isEnum)
      {
        enumType = Nullable.GetUnderlyingType(expr.Metadata.ModelType) ?? expr.Metadata.ModelType;
      }
      
      if (enumType != null)
      {
        enumCollectionItems = Enum.GetValues(enumType).Cast<Enum>()
          .Select(e => new SelectListItem(e.GetDescription(), e.ToString()))
          .ToArray();
      }

      if (modelType == typeof(string))
      {
        HtmlAttribute att = (HtmlAttribute)memberInfo[0].GetCustomAttributes(typeof(HtmlAttribute), false).FirstOrDefault();
        if (att != null)
          isHtml = true;

        matt = (MultiLineTextAttribute)memberInfo[0].GetCustomAttributes(typeof(MultiLineTextAttribute), false).FirstOrDefault();
        if (matt != null)
          isMultiLine = true;
      }

      if (!string.IsNullOrEmpty(cssClass))
        cssClass = " " + cssClass;

      string html = "";

      if (isBool)
      {
        html += $@"
<input type=""checkbox"" id=""{propName}"" v-model=""{propName}"" class=""form-check-input{editableClass}{cssClass}"" v-on:change=""clearValidation"" disabled{describedBy}> ";

        if (addLabel)
          html += $@" <label for=""{propName}"" class=""form-label"">{displayName}</label>";
      }
      else if (isEnum && radioListAtt != null)
      {
        html += $@"<fieldset>
<legend>{displayName}</legend>";

        foreach (var item in Enum.GetValues(enumType))
        {
          html += $@"
<div>
  <input type=""radio"" id=""{propName}{(int)item}"" name=""{propName}"" v-model=""{propName}"" class=""form-check-input{editableClass}{cssClass}"" v-on:change=""clearValidation"" value=""{item}"" disabled>
    <label for=""{propName}{(int)item}"" class=""form-label"">{WebUtility.HtmlEncode(item.GetDescription())}</label>
  </input>  
</div>";
        }

        html += @"
</fieldset>";
      }
      else
      {
        if (addLabel)
          html += $@"
<label for=""{propName}"" class=""form-label"">{displayName}</label>";

        if (isEnum)
        {
          html += $@"
<select id=""{propName}"" v-model=""{propName}"" class=""form-select{editableClass}{cssClass}"" v-on:change=""clearValidation"" disabled{describedBy}>";

          if (expr.Metadata.IsNullableValueType)
            html += $@"<option value="""">- Vælg -</option>";

          foreach (var item in Enum.GetValues(enumType).Cast<Enum>())
          {
            html += $@"
<option value=""{item}"">{WebUtility.HtmlEncode(item.GetDescription())}</option>";
          }

          html += @"
</select>";
        }
        else if (isDate)
        {
          html += $@"
<v-date-picker v-model=""{propName}"" v-on:change=""clearValidation"" mode=""date"" timezone=""UTC"" :model-config=""datePickerConfig"" :masks=""dateMasks"">
  <template v-slot=""{{ inputValue, inputEvents }}"">
    <input id=""{propName}"" class=""form-control"" autocomplete=""off"" :value=""inputValue"" v-on=""inputEvents"" :disabled=""!isEditing""/>
  </template>
</v-date-picker>";
        }
        else if (enumCollectionItems != null)
        {
          foreach (var item in enumCollectionItems)
          {
            html += $@"
<div class=""checkboxListItem"">
<input type=""checkbox"" id=""check{item.Value}"" value=""{item.Value}"" class=""form-check-input{editableClass}{cssClass}"" v-model=""{propName}"" :disabled=""!isEditing""/>
<label for=""check{item.Value}"">{item.Text}</label>
</div>
";
          }
        }
        else if (customEntities != null)
        {
          html += $@"
<select id=""{propName}"" v-model=""{propName}"" class=""form-select{editableClass}{cssClass}"" v-on:change=""clearValidation"" disabled{describedBy}>";
          if (expr.Metadata.IsNullableValueType)
            html += $@"<option value="""">- Vælg -</option>";

          foreach (var item in customEntities)
          {
            string activeStr = ((item.Model is IActiveState a) && !a.IsActive)
              ? " disabled"
              : "";

            html += $@"
<option value=""{item.CustomEntityId}""{activeStr}>{WebUtility.HtmlEncode(item.Title)}</option>";
          }

          html += @"
</select>";

        }
        else if (isMultiLine)
        {
          html += $@"
<textarea id=""{propName}"" v-model=""{propName}"" rows=""{matt.Rows}"" class=""form-control{editableClass}{cssClass}"" v-on:change=""clearValidation"" readonly{describedBy}></textarea>";
        }
        else if (isHtml)
        {
          html += $@"
<html-editor id=""{propName}"" height=""500""></html-editor>";
        }
        else if (isNumber)
        {
          html += $@"
<input type=""number"" id=""{propName}"" v-model.number=""{propName}"" class=""form-control{editableClass}{cssClass}"" v-on:change=""clearValidation"" readonly{describedBy}>";
        }
        else
        {
          html += $@"
<input type=""text"" id=""{propName}"" v-model=""{propName}"" class=""form-control{editableClass}{cssClass}"" v-on:change=""clearValidation"" readonly{describedBy}>";
        }
      }

      return html;
    }
  }
}
