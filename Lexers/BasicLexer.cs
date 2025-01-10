using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace PegSharp.Lexers;

public class BasicLexer : ILexer
{
    private static readonly Regex _wordPattern = new(@"[_a-zA-Z]+");
    private static readonly HashSet<char> _whitespaces = [' ', '\t', '\r', '\n'];
    private static readonly HashSet<char> _wordChars = [.."_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray()];
    private static readonly HashSet<char> _quoteChars = ['\'', '"'];
    private static readonly string[] _keywords = [
        "&&", "||",
        "!", "~", "^", "+", "-", "*", "/",
        ","
    ];

    private int _readCursor = 0;
    private Token[]? _tokens;

    public void Load(Stream stream)
    {
        using StreamReader reader = new(stream, encoding: Encoding.UTF8, leaveOpen: true);
        _tokens = Tokenize(reader);
    }

    public void Load(string source)
    {
        using StringReader reader = new(source);
        _tokens = Tokenize(reader);
    }

    private static Token[] Tokenize(TextReader reader)
    {
        List<Token> tokens = [];
        int lineNum = 0;
        string? line = reader.ReadLine();
        while (line != null)
        {
            string[] lineTokens = TokenizeLine(line);
            foreach (string lineToken in lineTokens)
                tokens.Add(new Token(lineToken, lineNum));
            lineNum++;
            line = reader.ReadLine();
        }
        return [.. tokens];
    }

    private static string[] TokenizeLine(string line)
    {
        List<string> tokens = [];
        TokenizerState state = new(line);
        while (state.TokenStart < line.Length)
        {
            if (state.TryBiteQuote(out string? quoteToken))
            {
                tokens.Add(quoteToken);
                continue;
            }
            if (_quoteChars.Contains(state.CurrentChar))
            {
                state.BeginQuote();
                continue;
            }
            state.SkipWhitespace();
            state.ExtendWhile(s => _wordChars.Contains(s.CurrentChar));
            state.TokenLength = state.TokenLength == 0 ? 1 : state.TokenLength;
            tokens.Add(state.Bite());
            state.SkipWhitespace();
            foreach (var keyword in _keywords)
            {
                if (line.StartsWith(keyword))
                {
                    tokens.Add(keyword);
                    state.TokenStart += keyword.Length;
                    state.TokenLength = 0;
                    break;
                }
            }
        }
        return [.. tokens];
    }

    public Token Read()
    {
        if (_tokens == null)
            throw new NullReferenceException("Load string to tokenize it before calling Read()");
        if (_readCursor >= _tokens.Length)
            return Token.EOF;
        var ret = _tokens[_readCursor];
        _readCursor++;
        return ret;
    }
}

internal record struct TokenizerState(string Str, int TokenStart = 0, int TokenLength = 0, char QuoteChar = '\0')
{
    public bool InQuote { get; private set; } = false;
    public readonly char CurrentChar => Str[TokenStart + TokenLength];
    public readonly string CurrentToken => Str.Substring(TokenStart, TokenLength);
    public readonly bool IsEOF => TokenStart + TokenLength == Str.Length;

    private void UncheckedSkip()
        => TokenStart++;
    private void UncheckedExtend()
        => TokenLength++;

    public void Skip()
    {
        if (TokenStart + 1 == Str.Length)
            throw new Exception($"Unexpected EOF when reading unfinished token [{CurrentToken}]");
        TokenStart++;
    }
    public void SkipWhile(Func<TokenizerState, bool> condition)
    {
        while (!IsEOF && condition(this))
            UncheckedSkip();
    }
    public void Extend()
    {
        if (IsEOF)
            throw new Exception($"Unexpected EOF when reading unfinished token [{CurrentToken}]");
        TokenLength++;
    }
    public void ExtendWhile(Func<TokenizerState, bool> condition)
    {
        while (!IsEOF && condition(this))
            UncheckedExtend();
    }
    public string Bite()
    {
        string token = CurrentToken;
        TokenStart += TokenLength;
        TokenLength = 0;
        return token;
    }
    public void SkipWhitespace()
        => SkipWhile(s => char.IsWhiteSpace(s.CurrentChar));
    public string BiteUntil(char charInclusive)
    {
        ExtendWhile(s => s.CurrentChar != charInclusive);
        return Bite();
    }
    public void BeginQuote()
    {
        InQuote = true;
        QuoteChar = CurrentChar;
        Extend();
    }
    public bool TryBiteQuote([NotNullWhen(true)]out string? token)
    {
        token = null;
        if (!InQuote)
            return false;
        token = BiteUntil(QuoteChar);
        InQuote = false;
        QuoteChar = '\0';
        return true;
    }
}
