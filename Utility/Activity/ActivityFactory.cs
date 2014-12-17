using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace CFlat.Utility
{
    public class ActivityFactory
    {
        private ActivityMap activityMap { get; set; }

        public void registerActivity(string verb, Type playgroundType, PlayerBase player)
        {
            ActivityFactoryRegistrationItem item = new ActivityFactoryRegistrationItem();
            item.player = player;
            item.playgroundType = playgroundType;
            activityMap.put(verb, item);
        }

        public void unregisterActivity(string verb)
        {
            activityMap.remove(verb);
        }

        public ActivityFactory()
        {
            activityMap = new ActivityMap();
        }

        public Activity createActivityFromJson(string jsonString)
        {
            Logger.debug("ActivityFactory: start construct activity from json : {0}", jsonString);
            var o = JsonConvert.DeserializeObject<dynamic>(jsonString);
            string verb = o.verb;

            ActivityFactoryRegistrationItem item = activityMap.search(verb);
            if (item == null)
            {
                return null;
            }
            MethodInfo staticMethod = item.playgroundType.GetMethod(
                "fromJson", 
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            MethodInfo typicalMethod = staticMethod.MakeGenericMethod(item.playgroundType);
            object[] paramList = new object[1];
            paramList[0] = jsonString;
            PlaygroundBase playgroundBase = typicalMethod.Invoke(null, paramList) as PlaygroundBase;
            Activity activity = new Activity();
            activity.player = item.player;
            activity.playground = playgroundBase;
            Logger.debug("ActivityFactory: activity constructed.");
            return activity;
        }
    }

}
