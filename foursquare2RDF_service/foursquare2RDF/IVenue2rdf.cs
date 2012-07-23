using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace foursquare2RDF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IVenue2rdf
    {
        // TODO: Add your service operations here
        [OperationContract]
       void addVenuesToGraph(string venue, string near);

    }

}
