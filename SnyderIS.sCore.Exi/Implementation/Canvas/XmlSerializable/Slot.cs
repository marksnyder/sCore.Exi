using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;
using SnyderIS.sCore.Exi.Interfaces.Canvas;


namespace SnyderIS.sCore.Exi.Implementation.Canvas.XmlSerializable
{
    [DataContract]
    public class Slot : ISlot
    {
        [DataMember(Name = "Identifier")]
        public Guid Identifier { get; set; }

        [DataMember(Name = "Title")]
        public string Title { get; set; }

        [DataMember(Name = "_DataSourceTypeName")]
        private string _DataSourceTypeName { get; set; }

        [DataMember(Name = "_RendererTypeName")]
        private string _RendererTypeName { get; set; }

        [DataMember(Name = "_Options")]
        private Dictionary<string, string> _Options { get; set; }

        [DataMember(Name = "PositionX")]
        public int X { get; set; }

        [DataMember(Name = "PositionY")]
        public int Y { get; set; }

        public System.Type DataSource
        {
            get
            {
                return System.Type.GetType(_DataSourceTypeName);
            }
        }

        public System.Type Renderer
        {
            get
            {
                return System.Type.GetType(_RendererTypeName);
            }
        }


        public IDictionary<string, string> Options
        {
            get
            {
                if (_Options == null)
                {
                    _Options = new Dictionary<string, string>();
                }

                return _Options;
            }
        }

        public void SetTypes(
            System.Type dataSource,
            System.Type renderer)
        {
            _DataSourceTypeName = dataSource.AssemblyQualifiedName;
            _RendererTypeName = renderer.AssemblyQualifiedName;
        }
    }
}
