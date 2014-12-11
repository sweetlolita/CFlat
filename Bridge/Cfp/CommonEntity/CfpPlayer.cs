using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CFlat.Utility;

namespace CFlat.Bridge.Cfp.CommonEntity
{
    public abstract class CfpPlayer : PlayerBase
    {
        public Guid sessionId { get; set; }
    }
}
