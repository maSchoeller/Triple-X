using System.Collections.Generic;

namespace TripleX.Prototype.Core.PaserHelpers
{
    public abstract record BaseToken();

    public record NumberToken(int Number) : BaseToken
    {
        public override string ToString()
        {
            return "" + Number;
        }
    }
    //public record WhitespaceToken() : BaseToken;
    public record SeperatorToken(char Seperator) : BaseToken
    {
        public override string ToString()
        {
            return "" + Seperator;
        }
    }
    public record PlusToken() : BaseToken
    {
        public override string ToString()
        {
            return "+";
        }
    }
    public record OpenBracketToken(char Bracket, bool isRound) : BaseToken;
    public record CloseBracketToken(char Bracket, bool isRound) : BaseToken;
    public record InvalidToken() : BaseToken;
    public record EndOfFileToken() : BaseToken;


    public abstract record TokenGroup() : BaseToken;

    public record NumberGroup(IList<BaseToken> Childs) : TokenGroup
    {
        public override string ToString()
        {
            string result = "";
            foreach (var token in Childs)
            {
                result += token;
            }
            return result;
        }
    }
    public record BracketGroup(TokenGroup Group, OpenBracketToken Open, CloseBracketToken Close) : TokenGroup
    {
        public override string ToString()
        {
            return Open.Bracket + Group.ToString() + Close.Bracket;
        }
    }

    public record PhoneNumber(TokenGroup? Country, TokenGroup Area, TokenGroup Main, TokenGroup? Forwarding);
}
