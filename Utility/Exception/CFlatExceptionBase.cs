using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFlat.Utility
{
    public class CFlatExceptionBase : Exception
    {
        public CFlatExceptionBase(string message)
            : base(message)
        {

        }
    }
}
