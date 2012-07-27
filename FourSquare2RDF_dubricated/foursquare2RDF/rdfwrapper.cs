using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF;
using VDS.RDF.Writing;
using System.IO;

namespace foursquare2RDF
{
    public class rdfwrapper
    {
        public string filepath;

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="filepath">file path to save RDF in</param>
        public rdfwrapper(string filepath)
        {
            this.filepath = filepath;
        }

        public void writeIntoFile (Graph g)
        {
            NTriplesWriter ntwriter = new NTriplesWriter();
            ntwriter.Save(g, filepath);
        }

        /// <summary>
        /// class destructor
        /// </summary>
        ~rdfwrapper()
        {
            
        }
    }
}
