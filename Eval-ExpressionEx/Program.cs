// See https://aka.ms/new-console-template for more information

using System.Security.Cryptography;
using System.Text;
using Z.Expressions;

Console.WriteLine("Hello, World!");

// alter
Eval.Execute("""Console.WriteLine("Hello, World!");""");


int x = 0;
var o = Order;
string MD5Encrypt32(string password = "")
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

int _customerCode = 1255;
string _appKey = "a1255yunlian20220426PEYGDJEY548654NCSOSHS";
long _timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

var t = new {
    sign = MD5Encrypt32(_customerCode + _appKey + _timestamp)
};
var exp = """ 
int _customerCode = 1255;
string _appKey = "a1255yunlian20220426PEYGDJEY548654NCSOSHS";
long _timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
string password = _customerCode + _appKey + _timestamp;
string sign = string.Empty;
foreach (byte num in MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(password)))
   sign += num.ToString("X2");

return new {
    sign
};
""".Compile<Func<object>>();

// if (x>1)
// {
//     var exp1 = """ 
// Order(x)
// Console.WriteLine(x);
// x;
// "sadsafewfwgwrgrw";
// """.Compile<Func<string>>();
// }
// else
// {
//     string json = "";
//     var exp2 = """ 
// Order(x)
// Console.WriteLine(x);
// x;
// """.Compile<Func<string,string>>("json");
//
//     var res = exp2(json);
//     
//     // 50%  50%难配
// }

var res = exp();

foreach (var item in res.GetType().GetProperties())
{
    Console.WriteLine(item.Name + "---" + item.GetValue(res));
}


void Order(int x)
{
    x += 100;
}


class VA
{
    public string a { get; set; }
    public string b { get; set; }
}