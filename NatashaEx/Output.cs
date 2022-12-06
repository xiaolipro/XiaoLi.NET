// using System;
// using System.Text;
//
// public static class N2aecb7d7b2a74d35acb29652041e1b0b
// {
//     public static System.Object Invoke()
//     {
//         int _customerCode = int.Parse(arg.customerCode);
//         string _appKey = arg.appKey;
//         long _timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
//         string pwd = _customerCode + _appKey + _timestamp;
//         string MD5Encrypt32(string password = "")
//         {
//             string empty = string.Empty;
//             try
//             {
//                 if (!string.IsNullOrEmpty(password) && !string.IsNullOrWhiteSpace(password))
//                 {
//                     foreach (byte num in System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(password)))
//                         empty += num.ToString("X2");
//                 }
//             }
//             catch
//             {
//                 throw new Exception("错误的 password 字符串:【" + password + "】");
//             }
//
//             return empty;
//         }
//
//         return new
//             {
//                 timeStamp = _timestamp.ToString(), sign = MD5Encrypt32(pwd), customerCode = _customerCode.ToString(), //body = arg2
//             }
//
//             ;
//     }
// }