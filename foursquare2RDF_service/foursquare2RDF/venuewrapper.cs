using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VDS.RDF;


namespace foursquare2RDF
{
    static class venuewrapper
    {
        public const string clientID = "SUFGRFKOSKRXC0AE35NHTIDAIGMQEZW4W43CAIAOM0QRDDT4";
        public const string clientSecret = "24RDEHJ0ARTUSS5EXKPFLIQCOFXKWPGHFLZ15H1CM5WDOV3E";

        /// <summary>
        /// search for venues in the foursquare API using placename and Place nearto it 
        /// </summary>
        /// <param name="placeName">place to search for</param>
        /// <param name="near">place to search near to </param>
        /// <returns>Jobject of the parser result </returns>
        public static JObject getVenues(string placeName, string near)
        {
            string queryString = queryBuild(placeName, near);


            string s = util.get(queryString);

            if (s != null)
            {
                JObject obj = JObject.Parse(s);
                return obj;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// search for venues in the foursquare API using longtitude and latitude
        /// </summary>
        /// <param name="placeName">place to search for</param>
        /// <param name="latitude">latitude  to search near to</param>
        /// <param name="longtitude">longtitude to search near to</param>
        /// <returns></returns>
        public static JObject getVenues(string placeName, float latitude, float longtitude)
        {
            string queryString = queryBuild(placeName, latitude, longtitude);


            string s = util.get(queryString);

            if (s != null)
            {
                JObject obj = JObject.Parse(s);
                return obj;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// build query part for the FourSquare Api 
        /// </summary>
        /// <param name="venue">venue name</param>
        /// <param name="near">place near to search in</param>
        /// <returns>string of the URL to be executed</returns>
        private static string queryBuild(string venue, string near)
        {
            string queryString = "https://api.foursquare.com/v2/venues/search" + "?near=" + near + "&query=" + venue + "&client_id=" + clientID + "&client_secret=" + clientSecret + "&v=20120101";
            return queryString;
        }

        /// <summary>
        /// build query part for the FourSquare Api 
        /// </summary>
        /// <param name="venue">venue name</param>
        /// <param name="longtitude">longtitude to search in</param>
        /// <param name="latitude">latitude to search in </param>
        /// <returns>string of the URL to be executed</returns>
        public static string queryBuild(string venue, float latitude, float longtitude )
        {
            string queryString = "https://api.foursquare.com/v2/venues/search" + "?ll=" + latitude + "," + longtitude + "&query=" + venue + "&client_id=" + clientID + "&client_secret=" + clientSecret + "&v=20120101";
            return queryString;
        }
    }
}
