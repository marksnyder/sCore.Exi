using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;

namespace SnyderIS.sCore.Exi.Implementation.Canvas.XmlSerializable
{
    [DataContract]
    internal class InternalData
    {
        [DataMember(Name = "Sections")]
        internal List<Section> Sections;
    }
}
