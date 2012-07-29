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
    /// <summary>
    /// the main functionality Class that contains the main logic , it implementes Ivenue2RDf interface for the websservice
    /// and it's responsible for adding new venues to the Graph or get the graph statistics or search for venues
    /// </summary>
    public class Venue2rdf : IVenue2rdf
    {
        Graph VenuesGraph;
        TripleStore VenuesTripleStore;
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
            VenuesTripleStore = new TripleStore();
            VenuesTripleStore.Add(VenuesGraph, true);

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
        public bool addVenuesToGraph(string venue, string near)
        {
            try
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

                if (VenuesTripleStore.HasGraph(VenuesGraph.BaseUri))
                    VenuesTripleStore.Remove(VenuesGraph.BaseUri);
                VenuesTripleStore.Add(VenuesGraph, false);
                rdfWrapper.writeGraphIntoFile(VenuesGraph);
                return true;
            }
            catch (Exception ex)
            {
                util.log("faled to add to venues RDF file using near property :" + ex.Message);
                return false;
            }
        }

        /// <summary>
        ///  search for venues in FrouSquare and add the special properties of the result to the Graph
        /// </summary>
        /// <param name="venue">keyword for the venue to search for</param>
        /// <param name="latitude">latitude to search near to</param>
        /// <param name="longtitude">longtitude to search near to</param>
        public bool addVenuesToGraphLL(string venue, float latitude, float longtitude)
        {
            try
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

                    VenuesGraph = assertSpecialPropertiesToGraph(VenuesGraph, S1, Tools.CopyNode(companyTriples[1].Subject, VenuesGraph));
                }

                if (VenuesTripleStore.HasGraph(VenuesGraph.BaseUri))
                    VenuesTripleStore.Remove(VenuesGraph.BaseUri);
                VenuesTripleStore.Add(VenuesGraph, true);
                rdfWrapper.writeGraphIntoFile(VenuesGraph);
                return true;
            }
            catch (Exception ex)
            {
                util.log("faled to add to venues RDF file using LL property :" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// get the latest statistics of the stored graph
        /// </summary>
        /// <returns>Json object contains the number of venues and categories and brands</returns>
        public string getStatistics()
        {
            string json = "{";

            try
            {
                //selecting the number of venyes
                Object results = VenuesTripleStore.ExecuteQuery("SELECT" +
                "  (count(distinct ?x) as ?venuesCount) " +
                "  (count(distinct ?y) as ?categoriesCount) " +
                "  (count(distinct ?z) as ?BrandsCount) " +
                "WHERE { " +
                "?z <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://dbpedia.org/ontology/Company>." +
                "optional {?x <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://foursquare2rdf/ontology/venue>}." +
                "optional {?y <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://foursquare2rdf/ontology/venuecategory>}" +
                "}"
                 );

                if (results is SparqlResultSet)
                {
                    //add venues count into 
                    SparqlResultSet rset = (SparqlResultSet)results;
                    foreach (SparqlResult result in rset)
                    {
                        json += "'venuesCount': '" + ((ILiteralNode)result.Value("venuesCount")).Value;
                        json += "' , ";
                        json += "'categoriesCount': '" + ((ILiteralNode)result.Value("categoriesCount")).Value;
                        json += "' , ";
                        json += "'BrandsCount': '" + ((ILiteralNode)result.Value("BrandsCount")).Value;
                        json += "'";
                    }
                }

                json += "}";

                return json;
            }
            catch (Exception ex)
            {
                util.log("can't get statistics:" + ex.Message);
                return "false";
            }
        }


        /// <summary>
        /// get all venues , categories , brands 
        /// basic implementation would be retrieving one brand name only for each search 
        /// due to complexity & size of returned json object would exceed limits FOR EX: 
        /// { Brand: { URI =  "uri " , Name = "name",
        /// Venues : [
        /// {"URI": "" , name : "" , longtitude : "" , latitude:""  , categories : [{    },{    },{    }]} , 
        /// {"URI": "" , name : "" , longtitude : "" , latitude:""  ,categories : [{    },{    },{     }]} 
        ///  ] }}
        ///  still Categories not implemented
        /// </summary>
        /// <param name="BrandName">search witha brand name , ex : nike</param>
        /// <returns>json string contains list of venues , categories , brand names </returns>
        public string getVenues(string BrandName, int limit = 1)
        {

            string query = "SELECT distinct " +
                "?brand ?label " +
                "WHERE { " +
                "?brand <" + typeProperty.Uri.ToString() + "> <http://dbpedia.org/ontology/Company>." +
                "?brand <" + labelProperty.Uri.ToString() + "> ?label ." +
                "?label <bif:contains> \"" + BrandName.Escape('\'').Trim() + "\"" +
                "FILTER(LANG(?label) = \"\" || LANGMATCHES(LANG(?label), \"en\"))" +
                "} limit " + limit;

            string json = "";
            try
            {


                //should query from local store , just there's issues with dotnetrdf  to be fixed !
                SparqlResultSet brandResultSet = util.executeSparqlQuery(query);

                //FOR EACH BRAND :
                foreach (SparqlResult brandResult in (SparqlResultSet)brandResultSet)
                {
                    //add the brand details :
                    json += "{'brand':{";
                    json += "'URI' : '" + ((IUriNode)brandResult.Value("brand")).ToString() + "'";
                    json += ",";
                    json += "'name' : '" + ((ILiteralNode)brandResult.Value("label")).Value + "'";


                    //getting venues underneath this brand 

                    query = "Select distinct " +
                        "?venue ?label ?long ?lat ?tips ?checkins ?users " +
                        "where {" +
                        "?venue <" + owningCompanyProperty + ">  <" + ((IUriNode)brandResult.Value("brand")).ToString() + ">. " +
                        "?venue  <" + labelProperty + "> ?label. " +
                        "?venue  <" + longtitudeProperty + "> ?long. " +
                        "?venue  <" + latitudeProperty + ">   ?lat. " +
                        "?venue  <" + longtitudeProperty + "> ?long . " +
                        "?venue  <" + checkCountProperty + "> ?checkins. " +
                        "?venue  <" + userCountProperty + ">  ?users. " +
                        "?venue  <" + tipCountProperty + ">  ?tips " +
                        "}";
                    //query = " select ?tips where { ?x <" + tipCountProperty + "> ?tips }";

                    Object venuesResultSet = VenuesTripleStore.ExecuteQuery(query);

                    json += " ,'venues':[";


                    if (venuesResultSet is SparqlResultSet && ((SparqlResultSet)venuesResultSet).Count > 0)
                    {

                        //FOR EACH BRAND:
                        foreach (SparqlResult venuesResult in (SparqlResultSet)venuesResultSet)
                        {
                            //collectingData 
                            string venueURI = ((IUriNode)venuesResult.Value("venue")).ToString();
                            string venuelabel = ((ILiteralNode)venuesResult.Value("label")).Value;
                            string venuelong = ((ILiteralNode)venuesResult.Value("long")).Value;
                            string venuelat = ((ILiteralNode)venuesResult.Value("lat")).Value;
                            string venuecheckins = ((ILiteralNode)venuesResult.Value("checkins")).Value;
                            string venueusers = ((ILiteralNode)venuesResult.Value("users")).Value;
                            string venuetips = ((ILiteralNode)venuesResult.Value("tips")).Value;


                            //add the Brand Details :
                            json += "{";
                            json += "\"URI\" :\"" + venueURI + "\"";
                            json += ",";
                            json += "\"name\" : \"" + venuelabel + "\"";
                            json += ",";
                            json += "\"longtitude\" : \"" + venuelong + "\"";
                            json += ",";
                            json += "\"latitude\" : \"" + venuelat + "\"";
                            json += ",";
                            json += "\"checkinCount\" : \"" + venuecheckins + "\"";
                            json += ",";
                            json += "\"userCount\" : \"" + venueusers + "\"";
                            json += ",";
                            json += "\"tipsCount\" : \"" + venuetips + "\"";
                            json += "},";
                        }

                        json = json.Remove(json.Length - 1);
                        json += "]}";
                        json += ",'notification':{'success':true}}";
                    }
                    else
                    {
                        json = "{'notification':{'success':false}}";
                    }

                    
                }

                
            }
            catch (Exception ex)
            {
                json = "{'notification':{'success':false}}";
                util.log(ex.Message);

            }

            return json;
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
        private Graph assertSpecialPropertiesToGraph(Graph g, JToken S1, INode ownerCompany)
        {
            //iterating over venues places found from foursquare API and adding special properties to the graph
            foreach (JToken element in S1)
            {
                IUriNode venueNode = VenuesGraph.CreateUriNode(new Uri("http://foursquare2RDF.com/Venue/" + element["id"]));
                //engaging the returned venue to the company name
                VenuesGraph.Assert(new Triple(venueNode, owningCompanyProperty, ownerCompany));

                //filling node labels
                g.Assert(new Triple(venueNode, typeProperty, VenuesGraph.CreateUriNode(new Uri("http://foursquare2rdf/ontology/venue"))));
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
                    g.Assert(new Triple(venueNode, subjectTypeProperty, venueCategory));
                    g.Assert(new Triple(venueCategory, labelProperty, VenuesGraph.CreateLiteralNode(category["name"].ToString())));
                    g.Assert(new Triple(venueCategory, labelPluralProperty, VenuesGraph.CreateLiteralNode(category["pluralName"].ToString())));
                    g.Assert(new Triple(venueCategory, labelShortProperty, VenuesGraph.CreateLiteralNode(category["shortName"].ToString())));
                    g.Assert(new Triple(venueCategory, typeProperty, VenuesGraph.CreateUriNode(new Uri("http://foursquare2rdf/ontology/venuecategory"))));

                }
            }

            return g;
        }

        #endregion

    }
}