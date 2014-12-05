using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFlat.Utility
{
    public abstract class ActivityBase
    {
        protected void validateParamCount(List<object> paramList, int count)
        {
            if (paramList.Count != count)
            {
                throw new CFlatExceptionBase("invalid param count.");
            }
        }

        protected void validateParamAsSpecificType(List<object> paramList, int index, Type type)
        {
            if (!paramList.ElementAt<object>(index).GetType().Equals(type))
            {
                throw new CFlatExceptionBase("invalid param type at " + index);
            }
        }
        abstract public void act(List<object> paramList);
    }
}
