using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CFlat.Utility
{
    public class Jsonable
    {
        protected Jsonable()
        {

        }

        public string toJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static T fromJson<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (System.Exception ex)
            {
                Logger.error("Jsonable: error: {0} | json: {1}", ex.Message, json);
                throw ex;
            }
            
        }
    }
}
