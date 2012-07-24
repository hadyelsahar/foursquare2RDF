using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using VDS.RDF;
using VDS.RDF.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace foursquare2RDF
{
    public class Venue2rdf : IVenue2rdf
    {
        Graph VenuesGraph;
        rdfwrapper rdfWrapper;
        #region OWLproperties
        public static IUriNode typeProperty;
        public static IUriNode labelProperty;
        public static IUriNode labelPluralProperty;
        public static IUriNode labelShortProperty;
        public static IUriNode latitudeProperty;
        public static IUriNode longtitudeProperty;
        public static IUriNode locationProperty;
        public static IUriNode checkCountProperty;
        public static IUriNode userCountProperty;
        public static IUriNode tipCountProperty;
        public static IUriNode countryProperty;
        public static IUriNode owningCompanyProperty;
        public static IUriNode subjectTypeProperty;
        #endregion

        /// <summary>
        /// class constructor , loading ontology into Venuesgraph
        /// </summary>
        public Venue2rdf()
        {
            rdfWrapper = new rdfwrapper("VenuesDB.rdf");
            VenuesGraph = rdfWrapper.readFileIntoGraph();

            #region properties Definition
            typeProperty = VenuesGraph.CreateUriNode(new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));
            labelProperty = VenuesGraph.CreateUriNode(new Uri("http://www.w3.org/2000/01/rdf-schema#label"));
            labelPluralProperty = VenuesGraph.CreateUriNode(new Uri("http://purl.org/net/vocab/2004/03/label#plural"));
            labelShortProperty = VenuesGraph.CreateUriNode(new Uri("http://purl.org/net/vocab/2004/03/label#short"));
            latitudeProperty = VenuesGraph.CreateUriNode(new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#lat"));
            longtitudeProperty = VenuesGraph.CreateUriNode(new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#long"));
            locationProperty = VenuesGraph.CreateUriNode(new Uri("http://dbpedia.org/ontology/location"));
            checkCountProperty = VenuesGraph.CreateUriNode(new Uri("http://foursquare2rdf/ontology/checkcount"));
            userCountProperty = VenuesGraph.CreateUriNode(new Uri("http://foursquare2rdf/ontology/usercount"));
            tipCountProperty = VenuesGraph.CreateUriNode(new Uri("http://foursquare2rdf/ontology/tipcount"));
            countryProperty = VenuesGraph.CreateUriNode(new Uri("http://dbpedia.org/property/locationCountry"));
            owningCompanyProperty = VenuesGraph.CreateUriNode(new Uri("http://dbpedia.org/ontology/owningCompany"));
            subjectTypeProperty = VenuesGraph.CreateUriNode(new Uri("http://purl.org/dc/terms/subject"));
            #endregion

        }

        /// <summary>
        /// search for venues in FrouSquare and add the special properties of the result to the Graph
        /// </summary>
        /// <param name="venue">keyword for the venue to search for</param>
        /// <param name="near">the keyword of the entity to search near to</param>
        public void addVenuesToGraph(string venue, string near)
        {

            List<Triple> companyTriples = rdfWrapper.getCompanyfromDBpedia(venue);

            //which means  one or more dbpedia owner company found
            if (companyTriples.Count >= 2)
            {

                JObject venuesObject = venuewrapper.getVenues(venue, near);
                JToken S1 = venuesObject["response"]["venues"];

                //adding the company properties extracted from dbpedia to the Graph
                foreach (Triple t in companyTriples)
                {
                    VenuesGraph.Assert(t);
                }

                VenuesGraph = assertSpecialPropertiesToGraph(VenuesGraph, S1, Tools.CopyNode(companyTriples[1].Subject, VenuesGraph));

            }

            rdfWrapper.writeGraphIntoFile(VenuesGraph);
        }

        /// <summary>
        ///  search for venues in FrouSquare and add the special properties of the result to the Graph
        /// </summary>
        /// <param name="venue">keyword for the venue to search for</param>
        /// <param name="latitude">latitude to search near to</param>
        /// <param name="longtitude">longtitude to search near to</param>
        public void addVenuesToGraphLL(string venue, float latitude, float longtitude)
        {
            List<Triple> companyTriples = rdfWrapper.getCompanyfromDBpedia(venue);

            if (companyTriples.Count >= 2)
            {
                JObject venuesObject = venuewrapper.getVenues(venue, latitude, longtitude);
                JToken S1 = venuesObject["response"]["venues"];

                //adding the company properties extracted from dbpedia to the Graph
                foreach (Triple t in companyTriples)
                {
                    VenuesGraph.Assert(t);
                }

                VenuesGraph = assertSpecialPropertiesToGraph(VenuesGraph, S1, Tools.CopyNode(companyTriples[1].Subject,VenuesGraph));
            }

            rdfWrapper.writeGraphIntoFile(VenuesGraph);

        }


        #region helpers

        /// <summary>
        /// a helper function to unify the code in the overloaded function , this method takes a graph g and a set of parser Venues 
        /// s1 and the owner company and add to the graph the special triples needed to be added to the graph , then returns the graph after the addition of special 
        /// properties
        /// </summary>
        /// <param name="g">the graph to add special triples into it</param>
        /// <param name="S1">Jtoken object of the venues array after being parsed by Newtonsoft Json parser</param>
        /// <param name="ownerCompany">the Inode of the owner company that was extracted from dbpedia to be joined with the found venues from the foursquare api</param>
        /// <returns>the new graph after addition of special triples</returns>
        public Graph assertSpecialPropertiesToGraph(Graph g , JToken S1 , INode ownerCompany)
        {
            //iterating over venues places found from foursquare API and adding special properties to the graph
            foreach (JToken element in S1)
            {
                IUriNode venueNode = VenuesGraph.CreateUriNode(new Uri("http://foursquare2RDF.com/Venue/" + element["id"]));
                //engaging the returned venue to the company name
                VenuesGraph.Assert(new Triple(venueNode, owningCompanyProperty, ownerCompany));

                //filling node labels 
                g.Assert(new Triple(venueNode, labelProperty, VenuesGraph.CreateLiteralNode(element["name"].ToString())));
                g.Assert(new Triple(venueNode, latitudeProperty, VenuesGraph.CreateLiteralNode(element["location"]["lat"].ToString())));
                g.Assert(new Triple(venueNode, longtitudeProperty, VenuesGraph.CreateLiteralNode(element["location"]["lng"].ToString())));
                g.Assert(new Triple(venueNode, countryProperty, VenuesGraph.CreateLiteralNode(element["location"]["country"].ToString())));
                g.Assert(new Triple(venueNode, checkCountProperty, VenuesGraph.CreateLiteralNode(element["stats"]["checkinsCount"].ToString())));
                g.Assert(new Triple(venueNode, tipCountProperty, VenuesGraph.CreateLiteralNode(element["stats"]["tipCount"].ToString())));
                g.Assert(new Triple(venueNode, userCountProperty, VenuesGraph.CreateLiteralNode(element["stats"]["usersCount"].ToString())));


            //iterating over categories for each venues
                foreach (JToken category in element["categories"])
                {
                    IUriNode venueCategory = VenuesGraph.CreateUriNode(new Uri("http://foursquare2RDF.com/VenueCategory/" + category["id"]));

                    //connect the venue to it's category 
                    g.Assert(new Triple(venueNode, subjectTypeProperty, venueCategory ));
                    g.Assert(new Triple(venueCategory, labelProperty, VenuesGraph.CreateLiteralNode(category["name"].ToString())));
                    g.Assert(new Triple(venueCategory, labelPluralProperty, VenuesGraph.CreateLiteralNode(category["pluralName"].ToString())));
                    g.Assert(new Triple(venueCategory, labelShortProperty, VenuesGraph.CreateLiteralNode(category["shortName"].ToString())));
                    g.Assert(new Triple(venueCategory, typeProperty, VenuesGraph.CreateUriNode(new Uri ("http://foursquare2rdf/ontology/venuecategory"))));

                }
            }

            return g; 
        }
        
        #endregion

    }
}
