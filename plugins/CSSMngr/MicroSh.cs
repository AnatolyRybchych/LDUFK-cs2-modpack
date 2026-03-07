

using System.Reflection.Metadata;
using System.Threading.Tasks.Sources;
using Microsoft.Extensions.Logging;

namespace CSSMngr;

public class MicroSh
{
    public delegate string Function(string input, string[] args);

    private enum TokenType
    {
        EOF,
        INVALID,
        SPACE,
        STRING,
        PIPE,
        DOLLAR,
        OPEN_PAREN,
        CLOSE_PAREN,
    };

    public Function Compile(Func<string, Function> getFunction, string expr)
    {
        ReadOnlySpan<char> source = expr;

        var funcChunks = parseFunction(getFunction, source, out source);
        var (funcName, isFuncNameStatic) = funcChunks[0];
        var funcArgs = funcChunks[1..];
    
        Function resultFunc = (string input, string[] args) => getFunction(funcName())(input, args);
        if (isFuncNameStatic)
            resultFunc = getFunction(funcName());

        ReadOnlySpan<char> token;
        while (skipSpaceGetToken(source, out token, out source) == TokenType.PIPE)
        {
            var pipeFuncChunks = parseFunction(getFunction, source, out source);
            var (pipeFuncName, isPipeFuncNameStatic) = pipeFuncChunks[0];
            Function pipeFunc = (string input, string[] args) => getFunction(pipeFuncName())(input, args);
            if (isPipeFuncNameStatic)
                pipeFunc = getFunction(pipeFuncName());

            var lhsArgs = ShortcutStrings(funcArgs).ToArray();
            funcArgs = pipeFuncChunks[1..];

            var prevResultFunc = resultFunc;
            resultFunc = (input, args) => pipeFunc(prevResultFunc(input, lhsArgs.Select((getter) => getter()).ToArray()), args);
        }

        if (source.Length != 0)
            throw new Exception($"Unexpected token '{token}'; Expected {TokenType.PIPE} or {TokenType.EOF}");

        if (funcArgs.Count == 0)
            return resultFunc;
        
        var curArgs = ShortcutStrings(funcArgs).ToArray();
        return (input, args) => resultFunc(input, curArgs.Select((getter) => getter()).ToArray().Concat(args).ToArray());
    }

    public List<(Func<string>, bool)> parseFunction(Func<string, Function> getFunction, ReadOnlySpan<char> source, out ReadOnlySpan<char> tail)
    {
        List<(Func<string>, bool)> result = [];
        ReadOnlySpan<char> token;
        tail = source;

        Func<string> funcName = ParseString(getFunction, tail, out bool isStatic, out tail)
                ?? throw new Exception($"Unexpected token {skipSpaceGetToken(source, out token, out source)}; Expected string");
        result.Add((funcName, isStatic));

        while (true)
        {
            Func<string>? chunk = ParseString(getFunction, tail, out isStatic, out tail);
            if (chunk == null)
                return result;

            result.Add((chunk, isStatic));
        }
    }

    private Func<string> ShortcutString(Func<string> str)
    {
        string value = str();
        return () => value;
    }

    private IEnumerable<Func<string>> ShortcutStrings(IEnumerable<(Func<string>, bool)> strs)
    {
        return strs.Select((maybeStatic) => maybeStatic.Item2 ? ShortcutString(maybeStatic.Item1) : maybeStatic.Item1);
    }

    private Func<string>? ParseString(Func<string, Function> getFunction, ReadOnlySpan<char> source, out bool isStatic, out ReadOnlySpan<char> tail)
    {
        List<Func<string>> chunkGetters = [];
        ReadOnlySpan<char> curTail = source;
        ReadOnlySpan<char> token;
        isStatic = true;

        while (true)
        {
            ReadOnlySpan<char> newTail = curTail;
            TokenType tokenType = getToken(curTail, out token, out newTail);
            if (tokenType == TokenType.STRING
            || tokenType == TokenType.DOLLAR
            || tokenType == TokenType.SPACE && chunkGetters.Count == 0)
                curTail = newTail;
            else            
                break;

            if (tokenType == TokenType.STRING)
            {
                string tokenVal = token.ToString();
                chunkGetters.Add(() => tokenVal);
            }
            else if(tokenType == TokenType.DOLLAR)
            {
                isStatic = false;
                if (skipSpaceGetToken(curTail, out token, out curTail) != TokenType.OPEN_PAREN)
                    throw new Exception($"Unexpected token {tokenType}; Expected {TokenType.OPEN_PAREN} as part of $(...) expression");
                
                var strings = parseFunction(getFunction, curTail, out curTail);
                var (funcName, isFuncNameStatic) = strings[0];

                if (skipSpaceGetToken(curTail, out token, out curTail) != TokenType.CLOSE_PAREN)
                    throw new Exception($"Unexpected token {tokenType}; Expected {TokenType.CLOSE_PAREN} as part of $(...) expression");

                var precalculatedArgs = ShortcutStrings(strings[1..]).ToArray();
                if (isFuncNameStatic)
                {   
                    var function = getFunction(funcName());
                    chunkGetters.Add(() => function("", precalculatedArgs.Select((argGetter) => argGetter()).ToArray()));
                }
                else
                {
                    chunkGetters.Add(() => getFunction(funcName())(
                        "", precalculatedArgs.Select((argGetter) => argGetter()).ToArray()));
                }
            }
        }

        if (chunkGetters.Count == 0)
        {
            tail = source[..];
            return null;
        }

        tail = curTail;

        if (chunkGetters.Count == 1)
            return chunkGetters.First();
        
        return () => string.Join("", chunkGetters.Select(getter => getter()));
    }

    private TokenType skipSpaceGetToken(ReadOnlySpan<char> source, out ReadOnlySpan<char> token, out ReadOnlySpan<char> tail)
    {
        TokenType type = getToken(source, out token, out tail);
        if (type != TokenType.SPACE)
            return type;

        return getToken(tail, out token, out tail);
    }

    private TokenType getToken(ReadOnlySpan<char> source, out ReadOnlySpan<char> token, out ReadOnlySpan<char> tail)
    {
        if (source.Length == 0)
        {
            tail = source[..];
            token = source[..];
            return TokenType.INVALID;
        }
        

        switch (source[0])
        {
            case '|':
                tail = source[1..];
                token = source[..1];
                return TokenType.PIPE;
            case '$':
                tail = source[1..];
                token = source[..1];
                return TokenType.DOLLAR;
            case '(':
                tail = source[1..];
                token = source[..1];
                return TokenType.OPEN_PAREN;
            case ')':
                tail = source[1..];
                token = source[..1];
                return TokenType.CLOSE_PAREN;
        }

        if (char.IsAsciiLetter(source[0]))
        {
            int cnt = 1;
            while (source.Length != cnt && (char.IsAsciiLetterOrDigit(source[cnt]) || source[cnt] == '.' || source[cnt] == '_'))
                cnt++;
            
            token = source[..cnt];
            tail = source[cnt..];
            return TokenType.STRING;
        }

        if (char.IsWhiteSpace(source[0]))
        {
            int cnt = 1;
            while (source.Length != cnt && (char.IsWhiteSpace(source[cnt])))
                cnt++;
            
            token = source[..cnt];
            tail = source[cnt..];
            return TokenType.SPACE;
        }

        token = source[..1];
        tail = source[1..];
        return TokenType.INVALID;
    }
}
