using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF;
using VDS.RDF.Writing;
using VDS.RDF.Parsing;
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


        /// <summary>
        /// read the stored RDF file and return it into a graph
        /// </summary>
        /// <returns></returns>
        public Graph readFileIntoGraph()
        {
            try
            {
                Graph g = new Graph();
                NTriplesParser ntparser = new NTriplesParser();

                //Load using Filename
                ntparser.Load(g, filepath);
                return g; 
            }
            catch (RdfParseException parseEx)
            {
                util.log("Parser Error");
                util.log(parseEx.Message);
            }
            catch (RdfException rdfEx)
            {
               util.log("RDF Error");
               util.log(rdfEx.Message);
            }

            return null ; 
        }

        /// <summary>
        /// function writes a graph into an RDF file
        /// </summary>
        /// <param name="g">graph to be written to rdf file</param>
        public void writeGraphIntoFile (Graph g)
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
