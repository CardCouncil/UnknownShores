using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnknownShores.Entities
{
    public class TokenModel
    {
        public string Token { get; set; }

        public string TokenType { get; set; }

        public string ExpiresIn { get; set; }
    }
}
