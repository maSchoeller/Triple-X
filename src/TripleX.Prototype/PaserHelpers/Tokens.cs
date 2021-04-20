using System.Collections;
using System.Collections.Generic;

namespace TripleX.Prototype.PaserHelpers
{
    public record BaseToken();

    public record NumberToken(int Number) : BaseToken;
    //public record WhitespaceToken() : BaseToken;
    public record SeperatorToken(char Seperator) : BaseToken;
    public record PlusToken() : BaseToken;
    public record OpenBracketToken(char Bracket) : BaseToken;
    public record CloseBracketToken(char Bracket) : BaseToken;
    public record InvalidToken() : BaseToken;
    public record EndOfFileToken() : BaseToken;


    public record TokenGroup() : BaseToken;

    public record NumberGroup(IList<BaseToken> Childs) : TokenGroup;
    public record BracketGroup(IList<BaseToken> Childs, OpenBracketToken Open, CloseBracketToken Close): TokenGroup;

    public record PhoneNumber(TokenGroup Country, TokenGroup Area, TokenGroup Main, TokenGroup Forwarding);
}
