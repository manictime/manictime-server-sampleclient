using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Finkit.ManicTime.Server.SampleClient.Resources
{
    public class TagCombinationListResource
    {
        public string[] TagCombinations { get; set; }
        public UserTagCombinationListResource[] UserTagCombinationLists { get; set; }
        public LinkResource[] Links { get; set; }
    }
}
