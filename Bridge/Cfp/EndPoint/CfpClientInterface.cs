using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CFlat.Utility;

namespace CFlat.Bridge.Cfp.EndPoint
{
    public interface CfpClientInterface
    {
        void send(PlaygroundBase playgroundBase);
    }
}
