using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using VDS.RDF;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace foursquare2RDF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Venue2rdf : IVenue2rdf     
    {
        Graph VenuesGraph;
        rdfwrapper rdfWrapper;

        /// <summary>
        /// class constructor , loading ontology into Venuesgraph
        /// </summary>
        public Venue2rdf()
        {
            rdfWrapper = new rdfwrapper("VenuesDB2.rdf");
            VenuesGraph = rdfWrapper.readFileIntoGraph();
        }


        /// <summary>
        /// search for venues in FrouSquare and add the special properties of the result to the Graph
        /// </summary>
        /// <param name="venue">keyword for the venue to search for</param>
        /// <param name="near">the keyword of the entity to search near to</param>
        public void addVenuesToGraph(string venue, string near)
        {
            #region properties Definition
            IUriNode typeProperty = VenuesGraph.CreateUriNode(new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));
            IUriNode labelProperty = VenuesGraph.CreateUriNode(new Uri("http://purl.org/net/vocab/2004/03/label#"));
            IUriNode labelPluralProperty = VenuesGraph.CreateUriNode(new Uri("http://purl.org/net/vocab/2004/03/label#plural"));
            IUriNode labelShortProperty = VenuesGraph.CreateUriNode(new Uri("http://purl.org/net/vocab/2004/03/label#short"));
            IUriNode latitudeProperty = VenuesGraph.CreateUriNode(new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#lat"));
            IUriNode longtitudeProperty = VenuesGraph.CreateUriNode(new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#long"));
            IUriNode locationProperty = VenuesGraph.CreateUriNode(new Uri("http://dbpedia.org/ontology/location"));
            IUriNode checkCountProperty = VenuesGraph.CreateUriNode(new Uri("http://foursquare2rdf/ontology/checkcount"));
            IUriNode userCountProperty = VenuesGraph.CreateUriNode(new Uri("http://foursquare2rdf/ontology/usercount"));
            IUriNode tipCountProperty = VenuesGraph.CreateUriNode(new Uri("http://foursquare2rdf/ontology/tipcount"));
            IUriNode countryProperty = VenuesGraph.CreateUriNode(new Uri("http://dbpedia.org/property/locationCountry"));
            #endregion

            JObject venuesObject = venuewrapper.getVenues(venue, near);            
            JToken S1 = venuesObject["response"]["venues"];

            foreach (JToken element in S1)
            {
                IUriNode venueNode = VenuesGraph.CreateUriNode(new Uri("http://foursquare2RDF.com/Venue/" + element["id"]));
                //ILiteralNode venueNameNode = g.CreateLiteralNode(element["name"].ToString());

                //filling node labels 
                VenuesGraph.Assert(new Triple(venueNode, labelProperty, VenuesGraph.CreateLiteralNode(element["name"].ToString())));
                VenuesGraph.Assert(new Triple(venueNode, latitudeProperty, VenuesGraph.CreateLiteralNode(element["location"]["lat"].ToString())));
                VenuesGraph.Assert(new Triple(venueNode, longtitudeProperty, VenuesGraph.CreateLiteralNode(element["location"]["lng"].ToString())));
                VenuesGraph.Assert(new Triple(venueNode, countryProperty, VenuesGraph.CreateLiteralNode(element["location"]["country"].ToString())));
                VenuesGraph.Assert(new Triple(venueNode, checkCountProperty, VenuesGraph.CreateLiteralNode(element["stats"]["checkinsCount"].ToString())));
                VenuesGraph.Assert(new Triple(venueNode, tipCountProperty, VenuesGraph.CreateLiteralNode(element["stats"]["tipCount"].ToString())));
                VenuesGraph.Assert(new Triple(venueNode, userCountProperty, VenuesGraph.CreateLiteralNode(element["stats"]["usersCount"].ToString())));

            }

            rdfWrapper.writeGraphIntoFile(VenuesGraph);


        }
    }
}
