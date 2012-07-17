using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Finkit.ManicTime.Server.SampleClient.Resources
{
    [Serializable]
    [DataContract(Name = "user")]
    public class UserResource
    {
        [XmlElement("userId")]
        [DataMember(Name = "userId")]
        public int UserId { get; set; }

        [XmlElement("username")]
        [DataMember(Name = "username")]
        public string Username { get; set; }
    }
}
