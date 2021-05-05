using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Threading;

using TripleX.Prototype.Core.PaserHelpers;

namespace TripleX.Core.Prototype
{
    public class PhonenumberParser
    {
        private readonly ImmutableArray<BaseToken> _tokens;
        private int _currentTokenIndex;
        private List<string> _errors;
        private string _phonenumber;

        public PhonenumberParser(string phonenumber)
        {
            _phonenumber = phonenumber;
            _errors = new();
            _tokens = ImmutableArray.Create(CreateToken(phonenumber).ToArray());
        }


        private BaseToken CurrentToken => Peek(0);

        private BaseToken Peek(int offset)
        {
            if (_tokens.Length <= _currentTokenIndex + offset)
            {
                return new InvalidToken();
            }
            return _tokens[_currentTokenIndex + offset];
        }

        private void NextToken() => _currentTokenIndex++;


        private TokenGroup GetCountryCode()
        {
            var group = GetPlusWithTwoDigit();
            if (group is not null)
            {
                return group;
            }
            if (CurrentToken is OpenBracketToken open2)
            {
                //Note: Usecase [+<two digits countrycode>], e.g. +49, +31, [+67], (+50)
                var open = CurrentToken;
                NextToken();
                group = GetPlusWithTwoDigit();
                if (group is not null)
                {
                    NextToken();
                    if (CurrentToken is CloseBracketToken close2 && close2.isRound == open2.isRound)
                    {
                        return new BracketGroup(group, open2, close2);
                    }
                }
                //Note: Usecase [00<two digits countrycode>], e.g.: 0049, 0031, [0057], (0075)
                var fourtokens = GetStartFourTokens();
                NextToken();
                if (CurrentToken is CloseBracketToken close && close.isRound == open2.isRound)
                {
                    return new BracketGroup(fourtokens, open2, close);
                }
                else
                {
                    throw new ArgumentException();
                }
            }
            else
            {
                return GetStartFourTokens();

            }



            throw new ArgumentException();

            TokenGroup? GetPlusWithTwoDigit()
            {
                if (CurrentToken is PlusToken)
                {
                    var plus = CurrentToken;
                    NextToken();
                    if (CurrentToken is NumberToken nr1)
                    {
                        NextToken();
                        if (CurrentToken is NumberToken nr2)
                        {
                            return new NumberGroup(new[] { plus, nr1, nr2 });
                        }
                    }

                }
                else
                {
                    return null;
                }
                throw new ArgumentException();
            }
            TokenGroup GetStartFourTokens()
            {
                if (CurrentToken is NumberToken { Number: 0 } nr1)
                {
                    NextToken();
                    var secondzero = CurrentToken;
                    if (secondzero is NumberToken { Number: 0 } nr2)
                    {
                        NextToken();
                        var number1 = CurrentToken;
                        NextToken();
                        var number2 = CurrentToken;
                        if (number1 is NumberToken nr3 && number2 is NumberToken nr4)
                        {
                            //TODO: Catch  usecase [0049]
                            return new NumberGroup(new[] { nr1, nr2, nr3, nr4 });
                        }
                        else
                        {
                            throw new ArgumentException();
                        }
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                }
                else
                {
                    throw new ArgumentException();
                }
            }
        }
        private TokenGroup GetAreaCode()
        {
            //Note: Area code must be 5 digits e.g.: (07443), [08256], 06234, 08235-, 6234
            if (CurrentToken is OpenBracketToken open)
            {
                NextToken();
                var list = new List<BaseToken>();
                var index = 0;
                while (CurrentToken is not CloseBracketToken)
                {
                    if (CurrentToken is not NumberToken)
                    {
                        throw new ArgumentException();
                    }
                    list.Add(CurrentToken);
                    NextToken();
                    index++;
                    if (index > 5)
                    {
                        throw new ArgumentException();
                    }
                }
                if (CurrentToken is CloseBracketToken close && close.isRound == open.isRound)
                {
                    return new BracketGroup(new NumberGroup(list), open, close);
                }
                throw new ArgumentException();
            }
            return GetNextFive();
            TokenGroup GetNextFive()
            {
                var list = new List<BaseToken>();
                if (CurrentToken is NumberToken{ Number: 0 })
                {
                    list.Add(CurrentToken);
                    NextToken();
                }
                for (int i = 0; i < 4; i++)
                {
                    if (CurrentToken is SeperatorToken)
                    {
                        break;
                    }
                    if (CurrentToken is not NumberToken)
                    {
                        throw new ArgumentException();
                    }
                    list.Add(CurrentToken);
                    NextToken();
                }
                return new NumberGroup(list);
            }
        }
        private TokenGroup GetMainGroup()
        {
            if (CurrentToken is SeperatorToken)
            {
                NextToken();
            }
            return new NumberGroup(GetNumberUntilSeperator().ToArray());

            IEnumerable<BaseToken> GetNumberUntilSeperator()
            {
                while (CurrentToken is NumberToken number)
                {
                    NextToken();
                    yield return number;
                }
            }
        }

        private TokenGroup GetForwardingGroup()
        {

            NextToken();
            if (CurrentToken is SeperatorToken)
            {
                NextToken();
            }
            return new NumberGroup(GetNumberUntilSeperator().ToArray());

            IEnumerable<BaseToken> GetNumberUntilSeperator()
            {
                while (CurrentToken is NumberToken number)
                {
                    NextToken();
                    yield return number;
                }
            }
        }
        public PhoneNumber GetPhonenumber()
        {
            TokenGroup? country = null;
            try
            {
                country = GetCountryCode();
                NextToken();
            }
            catch
            {
                _currentTokenIndex = 0;
            }
            TokenGroup area = GetAreaCode();
            TokenGroup main = GetMainGroup();
            TokenGroup forward = GetForwardingGroup();
            return new PhoneNumber(country, area, main, forward);
        }
        private IEnumerable<BaseToken> CreateToken(string phoneNumber)
        {
            phoneNumber = phoneNumber.Replace(" ", "");

            foreach (var character in phoneNumber)
            {
                if (character == '+')
                {
                    yield return new PlusToken();
                }
                else if (char.IsDigit(character))
                {
                    yield return new NumberToken((int)char.GetNumericValue(character));
                }
                else if (character is '/' or '-')
                {
                    yield return new SeperatorToken(character);
                }
                else if (character is '(' or '[')
                {
                    yield return new OpenBracketToken(character, character == '(');
                }
                else if (character is ')' or ']')
                {
                    yield return new CloseBracketToken(character, character == ')');
                }
                else
                {
                    yield return new InvalidToken();
                }
            }

            yield return new EndOfFileToken();
        }
    }
}
