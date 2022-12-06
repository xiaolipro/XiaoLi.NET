// See https://aka.ms/new-console-template for more information

using System.Dynamic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

NatashaInitializer.Preheating();
string script = """   
int _customerCode = int.Parse(arg.customerCode);
string _appKey = arg.appKey;
long _timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
string pwd = _customerCode + _appKey + _timestamp;

string MD5Encrypt32(string password = "")
{
    string empty = string.Empty;
    try
    {
        if (!string.IsNullOrEmpty(password) && !string.IsNullOrWhiteSpace(password))
        {
            foreach (byte num in System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(password)))
                empty += num.ToString("X2");
        }
    }
    catch
    {
        throw new Exception("错误的 password 字符串:【" + password + "】");
    }

    return empty;
}

return new {
    timeStamp = _timestamp.ToString(),
    sign = MD5Encrypt32(pwd),
    customerCode = _customerCode.ToString(),
    //body = arg2
};
""";

//script = "public static object A() { int _customerCode=int.Parse(arg[0]);string _appKey=arg[1];long _timestamp=new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();string pwd=_customerCode+_appKey+_timestamp;string MD5Encrypt32(string password=\"\") {     string empty = string.Empty;     try     {         if (!string.IsNullOrEmpty(password) && !string.IsNullOrWhiteSpace(password))         {             foreach (byte num in MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(password)))                 empty += num.ToString(\"X2\");         }     }     catch     {         throw new Exception(\"错误的 password 字符串:【\" + password + \"】\");     }      return empty; }  return new {     timeStamp = _timestamp.ToString(),     sign = MD5Encrypt32(pwd),     customerCode = _customerCode.ToString() };}";
var memberDeclaration = CSharpSyntaxTree.ParseText(script)
    .GetRoot()
    .DescendantNodes()
    .FirstOrDefault();
Console.WriteLine(memberDeclaration.NormalizeWhitespace().ToFullString());


var list = new List<(string name, string val)>();
list.Add(("customerCode", "1255"));
list.Add(("appKey", "a1255yunlian20220426PEYGDJEY548654NCSOSHS"));

var dd = NatashaManagement.CreateDomain("myDomain");
var nClass = NClass.UseDomain(dd)
    .Name("arg")
    .Public();

foreach (var item in list)
{
    //添加字段
    nClass.Property(fb => fb.Public().Static()
        .Name(item.name)
        .Type<string>()
        .OnlyGetter($"return \"{item.val}\";"));
}

var action = NDelegate.UseDomain(dd)
    //.ConfigUsing(nClass.GetType())
    .Func<object>(script);

var res = action();

foreach (var item in res.GetType().GetProperties())
{
    Console.WriteLine(item.Name + "---" + item.GetValue(res));
}

//如果以后都不会再用 action 可以卸载
action.DisposeDomain();
