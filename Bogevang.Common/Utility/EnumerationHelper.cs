﻿using System;
using System.ComponentModel;
using System.Reflection;

namespace Bogevang.Common.Utility
{
  public static class EnumerationHelper
  {
    // From https://stackoverflow.com/questions/479410/enum-tostring-with-user-friendly-strings
    public static string GetDescription<T>(this T enumerationValue)
    {
      Type type = enumerationValue.GetType();
      if (!type.IsEnum)
      {
        throw new ArgumentException($"EnumerationValue must be of Enum type - was {type}.", "enumerationValue");
      }

      // Tries to find a DescriptionAttribute for a potential friendly name for the enum
      MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
      if (memberInfo != null && memberInfo.Length > 0)
      {
        object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

        if (attrs != null && attrs.Length > 0)
        {
          // Pull out the description value
          return ((DescriptionAttribute)attrs[0]).Description;
        }
      }
      // If we have no description attribute, just return the ToString of the enum
      return enumerationValue.ToString();
    }
  }
}
