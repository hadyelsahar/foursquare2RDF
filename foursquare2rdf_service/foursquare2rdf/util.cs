using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using VDS.RDF.Query;
using System.IO;
using VDS.RDF.Storage;
using VDS.RDF.Parsing;
using System.Web;

namespace foursquare2RDF
{
    public static class util
    {

       
        /// <summary>
        /// send a get request to a specific URI
        /// </summary>
        /// <returns>string of the return message</returns>
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

        /// <summary>
        /// send a post request to a specific URI
        /// </summary>
        /// <returns>string of the return message</returns>
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

        /// <summary>
        /// do a HTTP request using a sparql query from a sparql endpoint stated in a file
        /// </summary>
        /// <param name="request">sparql Query to be executed</param>
        /// <returns>set of results resulted from executing the query</returns>
        public static SparqlResultSet executeSparqlQuery(string request)
        {
            SparqlResultSet toreturn = new SparqlResultSet();
            try
            {
                string path = HttpRuntime.BinDirectory + "endpointURI.txt";
                StreamReader sr = new StreamReader(path);
                string endpointURI = sr.ReadLine();
                SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri (endpointURI));
                sr.Close();
                endpoint.Timeout = 999999;
                toreturn = endpoint.QueryWithResultSet(request);
            }
            catch (Exception e)
            {

                util.log(request + e.Message + "==>" + e.Data);
            }
            return toreturn;

        }

        /// <summary>
        /// log text to the log file 
        /// </summary>
        /// <param name="s">string to be logged in the Logfile</param>
        public static void log(string s)
        {

            StreamWriter logWriter = File.AppendText(HttpRuntime.BinDirectory+"Log.txt");
            logWriter.Write(s + "\t" + DateTime.Now.ToLongTimeString().ToString() + "\n");
            logWriter.Close();
        }
        /// <summary>
        /// empty the log file 
        /// </summary>
        public static void clearLog()
        {
            FileStream fileStream = File.Open(HttpRuntime.BinDirectory+"/"+"Log.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fileStream.SetLength(0);
            fileStream.Flush();
            fileStream.Close();

        }

    }
}
