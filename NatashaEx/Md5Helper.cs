using System;
using System.Security.Cryptography;
using System.Text;

namespace MyNamespace;

public static class Md5Helper
{
    public static string MD5Encrypt32(string password = "")
    {
        string empty = string.Empty;
        try
        {
            if (!string.IsNullOrEmpty(password) && !string.IsNullOrWhiteSpace(password))
            {
                foreach (byte num in MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(password)))
                    empty += num.ToString("X2");
            }
        }
        catch
        {
            throw new Exception("错误的 password 字符串:【" + password + "】");
        }

        return empty;
    }
}