using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFlat.Bridge.Cfp.CommonEntity
{
    public class CfpException : Exception
    {
        public CfpException(string message)
            : base(message)
        {

        }
    }
}
