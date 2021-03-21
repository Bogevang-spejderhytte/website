using Bogevang.Common.Utility;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
      Expression<Func<TModel, TResult>> expression)
    {
      string html = $@"<div class=""row mb-3"">";

      string innerHtml = await helper.VueEditorColFor_string(expression);

      html += innerHtml + @"
</div>";

      return new HtmlString(html);
    }


    public static async Task<IHtmlContent> VueEditorColFor<TModel, TResult>(
      this IHtmlHelper<TModel> helper,
      Expression<Func<TModel, TResult>> expression)
    {
      string html = await helper.VueEditorColFor_string(expression);
      return new HtmlString(html);
    }


    public static async Task<string> VueEditorColFor_string<TModel,TResult>(
      this IHtmlHelper<TModel> helper,
      Expression<Func<TModel, TResult>> expression)
    {
      IModelExpressionProvider expressionProvider = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IModelExpressionProvider>();
      ModelExpression expr = expressionProvider.CreateModelExpression(helper.ViewData, expression);

      string propName = expr.Metadata.PropertyName;
      propName = Char.ToLowerInvariant(propName[0]) + propName.Substring(1);

      string displayName = expr.Metadata.DisplayName;
      string description = expr.Metadata.Description;

      Type modelType = expr.Metadata.ModelType;
      modelType = Nullable.GetUnderlyingType(modelType) ?? modelType;

      bool isBool = modelType == typeof(bool);
      bool isEnum = modelType.IsEnum;
      bool isDate = modelType.IsAssignableFrom(typeof(DateTime));

      List<KeyValuePair<string, string>> customEntities = null;

      if (modelType == typeof(int))
      {
        MemberInfo[] memberInfo = expr.Metadata.ContainerType.GetMember(expr.Metadata.PropertyName);
        CustomEntityAttribute att = (CustomEntityAttribute)memberInfo[0].GetCustomAttributes(typeof(CustomEntityAttribute), false).FirstOrDefault();
        if (att != null)
        {
          customEntities = (await helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IAdvancedContentRepository>()
            .CustomEntities()
            .GetByDefinitionCode(att.CustomEntityDefinitionCode)
            .AsRenderSummary()
            .MapItem(e => new KeyValuePair<string,string>(e.Title, e.CustomEntityId.ToString()))
            .ExecuteAsync()).ToList();
        }
      }

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

      if (isBool)
      {
        html += $@"
<input type=""checkbox"" id=""{propName}"" v-model=""{propName}"" class=""form-check-input editable"" v-on:change=""clearValidation"" readonly>
<label for=""{propName}"" class=""form-label"">{WebUtility.HtmlEncode(displayName)}</label>";
      }
      else
      {
        html += $@"
<label for=""{propName}"" class=""form-label"">{WebUtility.HtmlEncode(displayName)}</label>";

        if (isEnum)
        {
          SelectListItem[] items = Enum.GetValues(expr.Metadata.ModelType).Cast<Enum>()
            .Select(e => new SelectListItem(e.GetDescription(), e.ToString()))
          .ToArray();

          html += $@"
<select id=""{propName}"" v-model=""{propName}"" class=""form-select editable"" v-on:change=""clearValidation"" disabled>
<option>- Vælg -</option>";

          foreach (var item in Enum.GetValues(expr.Metadata.ModelType).Cast<Enum>())
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
<input type=""date"" id=""{propName}"" v-model=""{propName}"" class=""form-control editable"" v-on:change=""clearValidation"" readonly>";
        }
        else if (customEntities != null)
        {
          html += $@"
<select id=""{propName}"" v-model=""{propName}"" class=""form-select editable"" v-on:change=""clearValidation"" disabled>
<option>- Vælg -</option>";

          foreach (var item in customEntities)
          {
            html += $@"
<option value=""{item.Value}"">{WebUtility.HtmlEncode(item.Key)}</option>";
          }

          html += @"
</select>";

        }
        else
        {
          html += $@"
<input type=""text"" id=""{propName}"" v-model=""{propName}"" class=""form-control editable"" v-on:change=""clearValidation"" readonly>";
        }
      }

      html += descriptionHtml;

      html += $@"
  <div id=""{propName}_feedback"" class=""invalid-feedback""></div>
</div>";

      return html;
    }
  }
}
