using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace foursquare2RDF
{
    class Program
    {
        static void Main(string[] args)
        {
            string near = "cairo";
            string queryKW = "";
            string queryString = "https://api.foursquare.com/v2/venues/search" + "?near=" + near + "&query=" + queryKW + "&client_id=" + util.clientID + "&client_secret=" + util.clientSecret + "&v=20120101";
            string s = util.get(queryString);
            JObject obj = JObject.Parse(s);
            JToken S1 = obj["response"]["venues"];

            foreach (JToken element in S1)
            {
                Console.WriteLine(element["name"]);
            }

        }
    }
}
