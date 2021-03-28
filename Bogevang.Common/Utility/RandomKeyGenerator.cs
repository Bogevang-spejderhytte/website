using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Bogevang.Common.Utility
{
  public static class RandomKeyGenerator
  {
    internal static readonly char[] chars =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();


    // See https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings
    public static string GetRandomString(int size)
    {
      byte[] data = new byte[4 * size];
      using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
      {
        crypto.GetBytes(data);
      }

      StringBuilder result = new StringBuilder(size);
      for (int i = 0; i < size; i++)
      {
        var rnd = BitConverter.ToUInt32(data, i * 4);
        var idx = rnd % chars.Length;

        result.Append(chars[idx]);
      }

      return result.ToString();
    }
  }
}
