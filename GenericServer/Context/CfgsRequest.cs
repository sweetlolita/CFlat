using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFlat.GenericServer.Context
{
    public class CfgsRequest
    {
        public string business { get; set; }
        public string method { get; set; }
        public List<object> param { get; private set; }
        public CfgsRequest()
        {
            param = new List<object>();
        }
    }
}
