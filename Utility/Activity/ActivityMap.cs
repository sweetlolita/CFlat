using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFlat.Utility
{
    public class ActivityMap
    {
        private Dictionary<string, ActivityFactoryRegistrationItem> activityDict { get; set; }
        private object dictLocker { get; set; }
        public ActivityMap()
        {
            activityDict = new Dictionary<string, ActivityFactoryRegistrationItem>();
            dictLocker = new object();
        }

        internal void put(string verb, ActivityFactoryRegistrationItem item)
        {

            lock (dictLocker)
            {
                if (activityDict.ContainsKey(verb))
                {
                    activityDict[verb] = item;
                    Logger.debug("ActivityMap: activity of verb {0} has been updated.",
                        verb);
                }
                else
                {
                    activityDict.Add(verb, item);
                    Logger.debug("ActivityMap: activity of verb {0} has been added.",
                        verb);
                }
            }
  
        }

        internal ActivityFactoryRegistrationItem search(string verb)
        {
            ActivityFactoryRegistrationItem result = null;
            if (activityDict.TryGetValue(verb, out result) == false)
            {
                Logger.debug("ActivityMap: no Player for verb {0}.", verb);
            }
            return result;
        }

        public void remove(string verb)
        {

            lock (dictLocker)
            {
                if (activityDict.Remove(verb) == true)
                {
                    Logger.debug("ActivityMap: verb {0} removed.", verb);
                }
                else
                {
                    Logger.error("ActivityMap: verb {0} cannot be removed.", verb);
                }
            }
        }
    }
}
