using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace foursquare2RDF
{
    public static class util
    {
        public const string clientID = "SUFGRFKOSKRXC0AE35NHTIDAIGMQEZW4W43CAIAOM0QRDDT4";
        public const string clientSecret = "24RDEHJ0ARTUSS5EXKPFLIQCOFXKWPGHFLZ15H1CM5WDOV3E";
        
        //public const string oauthToken = "JMTEROO2BHMRRKPOGGUBRUBMTZKQY5A4WU4WH4LWC4VZF0BE";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string get(string url)
        {
           
            string responseString; 
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                 responseString = reader.ReadToEnd();
            }
            catch
            {
                responseString = null;
            }
            return responseString ;
        }

        public static string post(string url)
        {
            string responseString;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                responseString = reader.ReadToEnd();
            }
            catch
            {
                responseString = null;
            }
            return responseString;
        }
    }
}
