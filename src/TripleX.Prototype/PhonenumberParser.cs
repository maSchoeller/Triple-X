using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripleX.Prototype
{
    public class PhonenumberParser
    {
        public PhonenumberParser()
        {

        }

        public (string Country, string Area, string Main, string Forwarding, string ErrorMessage) Parser(string number)
        {
            return ("+49", "7443", "2899033", "235", "Kein error");
        }
    }
}
