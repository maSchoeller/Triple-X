using System;
using System.Collections.Generic;
using System.Linq;

using TripleX.Prototype.PaserHelpers;

namespace TripleX.Prototype
{
    public class PhonenumberParser
    {
        public PhonenumberParser()
        {

        }

        public (string Country, string Area, string Main, string Forwarding, string ErrorMessage) Parser(string number)
        {

            number = number.Replace(" ", "");
            var tokens = CreateToken(number).ToList();



            return ("+49", "7443", "2899033", "235", "Kein error");
        }



        private PhoneNumber CreatePhonenumber(IList<BaseToken> tokens)
        {
            var current = 0;
            BaseToken currentToken = Peek();
            TokenGroup country;
            TokenGroup area;
            TokenGroup main;
            TokenGroup forward;


            //Get CountryCode
            if (currentToken is PlusToken)
            {
                //Note: Usecase +<two digits countrycode>, e.g. +49, +31
                var number1 = NextToken();
                var number2 = NextToken();
                if (number1 is NumberToken && number2 is NumberToken)
                {
                    //TODO: Catch  usecase [+49]
                    country = new NumberGroup(new BaseToken[] { currentToken, number1, number2 });
                }
                else
                {
                    throw new ArgumentException();
                }
            }
            else if (currentToken is NumberToken { Number: 0 })
            {
                //Note: Usecase 00<two digits countrycode>, e.g.: 0049, 0031
                var nexttoken = NextToken();
                if (nexttoken is NumberToken { Number: 0 })
                {
                    var number1 = NextToken();
                    var number2 = NextToken();
                    if (number1 is NumberToken && number2 is NumberToken)
                    {
                        //TODO: Catch  usecase [0049]
                        country = new NumberGroup(new BaseToken[] { currentToken, number1, number2 });
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                }
            }
            currentToken = NextToken();
            if (currentToken is NumberToken)
            {
                //TODO: Area code 
            }



            BaseToken NextToken()
            {
                current++;
                return Peek();
            }

            BaseToken Peek(int offset = 0)
            {
                if (tokens.Count > current + offset)
                {
                    return new InvalidToken();
                }
                return tokens[current + offset];
            }
            return new PhoneNumber(country, area, main, forward);
        }


        private IEnumerable<BaseToken> CreateToken(string phoneNumber)
        {
            foreach (var character in phoneNumber)
            {
                if (character == '+')
                {
                    yield return new PlusToken();
                }
                if (char.IsDigit(character))
                {
                    yield return new NumberToken(Convert.ToInt32(character));
                }
                if (character is '/' or '-')
                {
                    yield return new SeperatorToken(character);
                }
                if (character is '(' or '[')
                {
                    yield return new OpenBracketToken(character);
                }
                if (character is ')' or ']')
                {
                    yield return new CloseBracketToken(character);
                }
                yield return new InvalidToken();
            }

            yield return new EndOfFileToken();
        }
    }
}
