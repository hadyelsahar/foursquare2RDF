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

        public rdfwrapper(string filepath)
        {
            this.filepath = filepath;
            //if (!File.Exists(filepath))
            //{
            //    File.Create(filepath);
            //}
        }

        public void writeIntoFile (Graph g)
        {
            NTriplesWriter ntwriter = new NTriplesWriter();
            ntwriter.Save(g, filepath);
        }
    }
}
