using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VDS.RDF;


namespace foursquare2RDF
{
    static  class venuewrapper
    {
        public const string clientID = "SUFGRFKOSKRXC0AE35NHTIDAIGMQEZW4W43CAIAOM0QRDDT4";
        public const string clientSecret = "24RDEHJ0ARTUSS5EXKPFLIQCOFXKWPGHFLZ15H1CM5WDOV3E";

   
        public static JObject getVenues(string near, string placeName)
        {
            string queryString = "https://api.foursquare.com/v2/venues/search" + "?near=" + near + "&query=" + placeName + "&client_id=" + clientID + "&client_secret=" + clientSecret + "&v=20120101";
            string s = util.get(queryString);
            JObject obj = JObject.Parse(s);

            return obj;
            //JToken S1 = obj["response"]["venues"];
            //foreach (JToken element in S1)
            //{
            //    Console.WriteLine(element["name"]);
            //}
        }
    }
}
