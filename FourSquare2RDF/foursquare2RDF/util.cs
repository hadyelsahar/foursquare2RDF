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


        /// <summary>
        /// log text to the log file 
        /// </summary>
        /// <param name="s">string to be logged in the Logfile</param>
        public static void log(string s)
        {

            StreamWriter logWriter = File.AppendText(@".\Log.txt");
            logWriter.Write(s + "\t" + DateTime.Now.ToLongTimeString().ToString() + "\n");
            logWriter.Close();
        }
        /// <summary>
        /// empty the log file 
        /// </summary>
        public static void clearLog()
        {
            FileStream fileStream = File.Open(@".\Log.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fileStream.SetLength(0);
            fileStream.Flush();
            fileStream.Close();

        }

    }
}
