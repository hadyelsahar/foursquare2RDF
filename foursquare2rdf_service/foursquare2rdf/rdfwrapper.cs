﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF;
using VDS.RDF.Writing;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using System.IO;
using System.Web;

namespace foursquare2RDF
{
    /// <summary>
    /// is the class that is responsible for any interactions with LinkedData reading from or writing to ( those in the RDF file or in remote Triple Stores like Dbpedia )
    /// the class is initialized for each filepath so each class would interface only one local triple store
    /// </summary>
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
                ntparser.Load(g, HttpRuntime.BinDirectory+"/"+ filepath);
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

            return null;
        }

        /// <summary>
        /// function writes a graph into an RDF file
        /// </summary>
        /// <param name="g">graph to be written to rdf file</param>
        public void writeGraphIntoFile(Graph g)
        {
            try
            {
                NTriplesWriter ntwriter = new NTriplesWriter();
                ntwriter.Save(g, HttpRuntime.BinDirectory + "/" + filepath);
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
        }

        /// <summary>
        /// querying for companies from a triple store , return the result in the form of 
        /// </summary>
        public List<Triple> getCompanyfromDBpedia(string company, int limit = 1)
        {

            string query = "select distinct ?subject ?type ?literal where{ " +
                    "?subject <http://www.w3.org/2000/01/rdf-schema#label> ?literal. " +
                    "?subject <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> ?type ." +
                    "Filter ( " +
                    "("+
                    "?type = <http://dbpedia.org/resource/Public_company> || " +
                    "?type= <http://dbpedia.org/resource/Aktiengesellschaft> || " +
                    "?type= <http://dbpedia.org/ontology/Company>" +
                    ") && "+
                    "(LANG(?literal) = \"\" || LANGMATCHES(LANG(?literal), \"en\"))" +
                    ")" +
                    "?literal bif:contains '\"" + company.Escape('\'') + "\"'.} limit " + limit + "";

            SparqlResultSet resultSet = util.executeSparqlQuery(query);

            List<Triple> companyTriples = new List<Triple>();
            Graph g = new Graph();
            foreach (SparqlResult result in resultSet)
            {
                UriNode companyNode = (UriNode)result.Value("subject");
                UriNode typeNode = (UriNode)result.Value("type");
                LiteralNode labelNode = (LiteralNode)result.Value("literal");

                companyTriples.Add(new Triple(Tools.CopyNode(companyNode, g), Tools.CopyNode(Venue2rdf.typeProperty,g), Tools.CopyNode(typeNode, g)));
                companyTriples.Add(new Triple(Tools.CopyNode(companyNode, g), Tools.CopyNode(Venue2rdf.labelProperty,g), Tools.CopyNode(labelNode, g)));

            }

            return companyTriples;
        }


        /// <summary>
        /// class destructor
        /// </summary>
        ~rdfwrapper()
        {

        }
    }
}
