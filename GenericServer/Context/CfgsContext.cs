using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CFlat.Utility;

namespace CFlat.GenericServer.Context
{
    public class CfgsContext
    {
        public SingleListServerInterface<CfgsContext> server { get; set; }
        public CfgsRequest request { get; private set; }
        public CfgsResponse response { get; private set; }
        public CfgsContext(CfgsRequest request, CfgsResponse response)
        {
            this.request = request;
            this.response = response;
        }
    }
}
