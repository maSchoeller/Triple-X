using System;

using TripleX.Core.Prototype;

using Xunit;

namespace TripleX.Prototype.Tests
{
    public class PhonenumberTests
    {
        [Theory]
        [InlineData("+49 (941) 790-4780")]
        [InlineData("0033 0201 / 123456")]
        [InlineData("+49 0201 123456")]
        [InlineData("+44 0201123456")]
        [InlineData("0049201123456")]
        [InlineData("015115011900")]
        [InlineData("+91 09870987 899")]
        [InlineData("+49 (8024) [990-477]")]
        //[InlineData("(0)201 1234 56")]
        //[InlineData("[+49] (0)89-800/849-50")]
        public void CheckPhonenumber(string phonenumber)
        {
            var analyser = new PhonenumberParser(phonenumber);
            analyser.GetPhonenumber();
        }
    }
}
