using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.ServiceModel.Activation;

namespace foursquare2RDF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IVenue2rdf
    {

        // TODO: Add your service operations here
        [OperationContract]
        [WebGet]
        bool addVenuesToGraph(string venue, string near);

        [OperationContract]
        [WebGet]
        bool addVenuesToGraphLL(string venue, float longtitude, float latitude);

        [OperationContract]
        [WebGet]
        string getStatistics();


        [OperationContract]
        [WebGet]
        string getVenues(string BrandName, int limit);
    }
}
