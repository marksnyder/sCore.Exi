﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;

namespace SnyderIS.sCore.Exi.Implementation.Canvas.XmlSerializable.Storage
{
    [DataContract]
    internal class SharedData
    {
        [DataMember(Name = "SharedSection")]
        internal Section SharedSection;
    }
}
