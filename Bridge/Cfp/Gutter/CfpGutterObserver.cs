using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CFlat.Utility;

namespace CFlat.Bridge.Cfp.Gutter
{
    public interface CfpGutterObserver
    {
        void onCfpActivity(Activity activity);
        void onCfpConnected(Guid sessionId);
        void onCfpDisconnected(Guid sessionId);
        void onCfpError(Guid sessionId, string errorMessage);
    }
}
