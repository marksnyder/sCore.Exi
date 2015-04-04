using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;

namespace SnyderIS.sCore.Exi.Implementation.Canvas.XmlSerializable.Storage
{
    [DataContract]
    internal class PrivateData
    {
        [DataMember(Name = "PrivateSections")]
        internal List<Section> PrivateSections;

        [DataMember(Name = "DefaultSection")]
        internal Guid DefaultSection;

    }
}
