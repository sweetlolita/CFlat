using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFlat.Utility
{
    public interface SingleListServerInterface<T>
    {
        void postRequest(T request);
    }
}
