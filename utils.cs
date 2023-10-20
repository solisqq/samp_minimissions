using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace partymode
{
    internal class utils
    {
        public static string createFlatJson(Dictionary<string, object> data)
        {
            string toRet = "";
            foreach (var item in data)
            {
                toRet += '"' + item.Key + "\": " + '"' + item.Value.ToString() + "\",";
            }
            if (toRet.Length > 0) toRet = toRet.Remove(toRet.Length - 1);
            return toRet;
        }
        public static List<SampSharp.GameMode.Vector3> flatToVectorList(string data)
        {
            List<SampSharp.GameMode.Vector3> toRet = new List<SampSharp.GameMode.Vector3>();
            var splitted = data.Split('&');
            foreach (var item in splitted)
            {
                try
                {
                    var items = item.Split(',');
                    if (items.Length == 3)
                    {
                        toRet.Add(
                            new SampSharp.GameMode.Vector3(
                                (float)Convert.ToDouble(items[0], CultureInfo.InvariantCulture),
                                (float)Convert.ToDouble(items[1], CultureInfo.InvariantCulture),
                                (float)Convert.ToDouble(items[2], CultureInfo.InvariantCulture)));
                    }
                }
                catch { }
            }
            return toRet;
        }
        public static List<string[]> unpackData(string data, uint itemsInRow)
        {
            List<string[]> toRet = new List<string[]>();
            var splitted = data.Split('&');
            foreach (var item in splitted)
            {
                try
                {
                    var items = item.Split(',');
                    if (items.Length == itemsInRow)
                        toRet.Add(items);
                }
                catch { }
            }
            return toRet;
        }
        public static string getRandomFromFlat(string data, char splitter)
        {
            var splitted = data.Split(splitter);
            if (splitted.Length <= 1) return data;
            Random random = new Random();
            return splitted[random.Next(0, splitted.Length)];
        }
        public static Tuple<string, int, int> splitEveryNOnSpaces(string input, string delimiter, int n)
        {
            string result = "";
            int i = 0;
            int occ = 1;
            int maxLength = 0;
            int currentLength = 0;
            foreach(char c in input)
            {
                if (c == ' ' && i >= n - 3)
                {
                    result += "~n~";
                    i = 0;
                    occ++;
                    if(currentLength> maxLength) maxLength = currentLength;
                    currentLength = 0;
                    continue;
                }
                currentLength++;
                result += c;
                i++;
            }
            if (currentLength > maxLength) maxLength = currentLength;

            return new Tuple<string, int, int>(result, occ, maxLength);
        }
    }
}
