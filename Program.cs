using PegSharp;
using PegSharp.Lexers;
using PegSharp.Grammar;

// atom = \".+?\"
//

Regex ATOM = @"([a-zA-Z][a-zA-Z0-9]*)|(\.)";
var SEXP = new Temp();
var LIST = ("(" - SEXP - ")") / ("(" - SEXP - +("," - SEXP) - ")");
SEXP.Install(ATOM / LIST);

BasicLexer lexer = new();
while (true)
{
    Console.Write("> ");
    string? input = Console.ReadLine();
    if (input == null)
        continue;
    lexer.Load(input);
    var parsed = SEXP.Parse(lexer);
    if (parsed == null)
    {
        Console.WriteLine("<Failed to parse>");
        continue;
    }
    Console.WriteLine(parsed.AsString());
}
