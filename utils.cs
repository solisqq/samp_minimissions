using System;
using System.Collections.Generic;
using System.Text;

namespace partymode
{
    internal class utils
    {
        public static string createFlatJson(Dictionary<string, object> data)
        {
            string toRet = "";
            foreach(var item in data)
            {
                toRet += '"' + item.Key + "\": " + '"' + item.Value.ToString() + "\",";
            }
            if (toRet.Length>0) toRet = toRet.Remove(toRet.Length-1);
            return toRet;
        }
    }
}
