using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFlat.Utility
{
    public class ActivityMap
    {
        private Dictionary<string, ActivityBase> activityDict { get; set; }
        private object dictLocker { get; set; }
        public ActivityMap()
        {
            activityDict = new Dictionary<string, ActivityBase>();
            dictLocker = new object();
        }

        public void put(string verb, ActivityBase registeredActivity)
        {
            lock (dictLocker)
            {
                if (activityDict.ContainsKey(verb))
                {
                    activityDict[verb] = registeredActivity;
                    Logger.debug("ActivityMap: activity of verb {0} has been updated.",
                        verb);
                }
                else
                {
                    activityDict.Add(verb, registeredActivity);
                    Logger.debug("ActivityMap: activity of verb {0} has been added.",
                        verb);
                }
            }
        }

        public ActivityBase search(string verb)
        {
            ActivityBase result = null;
            if (activityDict.TryGetValue(verb, out result) == false)
            {
                Logger.debug("ActivityMap: no activity for verb {0}.", verb);
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
