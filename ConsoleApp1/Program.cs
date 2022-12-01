// See https://aka.ms/new-console-template for more information

using Z.Expressions;

Console.WriteLine("Hello, World!");

// alter
Eval.Execute("""Console.WriteLine("Hello, World!");""");


int x = 0;
var o = Order;
var exp = """ 
Order(x)
Console.WriteLine(x);
x;
""".Compile<Func<Action<int>, int, int>>("o", "x");


if (x>1)
{
    var exp1 = """ 
Order(x)
Console.WriteLine(x);
x;
"sadsafewfwgwrgrw";
""".Compile<Func<string>>();
}
else
{
    string json = "";
    var exp2 = """ 
Order(x)
Console.WriteLine(x);
x;
""".Compile<Func<string,string>>("json");

    var res = exp2(json);
    
    // 50%  50%难配
}

x = exp(o, x);


Console.WriteLine(x);

void Order(int x)
{
    x += 100;
}