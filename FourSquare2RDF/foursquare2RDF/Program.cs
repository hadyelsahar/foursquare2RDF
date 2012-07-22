using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VDS.RDF;

namespace foursquare2RDF
{
    class Program
    {
    

        static void Main(string[] args)
        {
         JObject venuesObject =  venuewrapper.getVenues("cairo", "macdonalds");
         Graph g = new Graph();

         #region properties Definition
         IUriNode typeProperty = g.CreateUriNode(new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"));
         IUriNode labelProperty = g.CreateUriNode(new Uri ("http://purl.org/net/vocab/2004/03/label#"));
         IUriNode labelPluralProperty = g.CreateUriNode(new Uri ("http://purl.org/net/vocab/2004/03/label#plural"));
         IUriNode labelShortProperty = g.CreateUriNode(new Uri ("http://purl.org/net/vocab/2004/03/label#short"));
         IUriNode latitudeProperty = g.CreateUriNode(new Uri ("http://www.w3.org/2003/01/geo/wgs84_pos#lat"));
         IUriNode longtitudeProperty = g.CreateUriNode(new Uri ("http://www.w3.org/2003/01/geo/wgs84_pos#long"));
         IUriNode locationProperty = g.CreateUriNode(new Uri ("http://dbpedia.org/ontology/location"));
         IUriNode checkCountProperty = g.CreateUriNode(new Uri ("http://foursquare2rdf/ontology/checkcount"));
         IUriNode userCountProperty = g.CreateUriNode(new Uri ("http://foursquare2rdf/ontology/usercount"));
         IUriNode tipCountProperty = g.CreateUriNode(new Uri ("http://foursquare2rdf/ontology/tipcount"));
         IUriNode countryProperty = g.CreateUriNode(new Uri ("http://dbpedia.org/property/locationCountry"));
         #endregion

         rdfwrapper rdfWrapper = new rdfwrapper("VenuesDB2.rdf");
         JToken S1 = venuesObject["response"]["venues"];

         foreach (JToken element in S1)
         {
             IUriNode venueNode = g.CreateUriNode(new Uri("http://foursquare2RDF.com/Venue/"+ element["id"]));
             //ILiteralNode venueNameNode = g.CreateLiteralNode(element["name"].ToString());
             

             //filling node labels 
             g.Assert(new Triple(venueNode, labelProperty, g.CreateLiteralNode(element["name"].ToString())));
             g.Assert(new Triple (venueNode ,latitudeProperty , g.CreateLiteralNode(element["location"]["lat"].ToString())));
             g.Assert(new Triple (venueNode ,longtitudeProperty , g.CreateLiteralNode(element["location"]["lng"].ToString())));
             g.Assert(new Triple (venueNode ,countryProperty, g.CreateLiteralNode(element["location"]["country"].ToString())));
             g.Assert(new Triple(venueNode, checkCountProperty, g.CreateLiteralNode(element["stats"]["checkinsCount"].ToString())));
             g.Assert(new Triple(venueNode, tipCountProperty, g.CreateLiteralNode(element["stats"]["tipCount"].ToString())));
             g.Assert(new Triple(venueNode, userCountProperty, g.CreateLiteralNode(element["stats"]["usersCount"].ToString())));
             
         }

         rdfWrapper.writeIntoFile(g);
        }
    }
}
