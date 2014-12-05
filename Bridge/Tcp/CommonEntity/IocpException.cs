using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFlat.Bridge.Tcp
{
    public class IocpException : Exception
    {
        public IocpException(string message) : base(message)
        {

        }
    }
}
