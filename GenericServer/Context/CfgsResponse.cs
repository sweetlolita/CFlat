using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFlat.GenericServer.Context
{
    public abstract class CfgsResponse
    {
        public bool isSuccess { get; set; }
        public string errorMessage { get; set; }
        public List<object> result { get; private set; }
        public CfgsResponse()
        {
            result = new List<object>();
        }
        public abstract void onResponsed();
    }
}
