using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;
using SnyderIS.sCore.Exi.Interfaces.Canvas;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SnyderIS.sCore.Exi.Implementation.Canvas.XmlSerializable
{

    public abstract class Canvas : ICanvas
    {

        private List<Section> _Sections;
        private Guid _DefaultSection;

        public abstract void UpdatePrivate(string data);
        public abstract void UpdateShared(string data, Guid id);

        public IEnumerable<ISection> GetSections()
        {
            EnsureSections();
            return _Sections;
        }

        public void RemoveSection(Guid id)
        {
            EnsureSections();

            var match = _Sections.Where(x => x.Identifier == id).FirstOrDefault();
            
            if (match != null)
            {
                _Sections.Remove(match);
                Serialize();
            }
        }

        public void RemoveSlot(Guid id)
        {
            EnsureSections();

            foreach (var section in _Sections)
            {
                foreach (var colum in section.Columns)
                {
                    var match = colum.Slots.Where(x => x.Identifier == id).FirstOrDefault();

                    if (match != null)
                    {
                        section.RemoveSlot(id);
                        Serialize();
                    }
                }
            }
        }

        public void MoveSlot(Guid id, int newPosition, int newColumn, int newX, int newY)
        {
            EnsureSections();

            foreach (var section in _Sections)
            {
                foreach (var column in section.Columns)
                {
                    var match = column.Slots.Where(x => x.Identifier == id).FirstOrDefault();

                    if (match != null)
                    {
                        section.MoveSlot(id, newPosition, newColumn,newX, newY);
                        Serialize();
                    }
                }
            }
        }

        public void RenameSlot(Guid id, string newName)
        {
            EnsureSections();

            foreach (var section in _Sections)
            {
                foreach (var column in section.Columns)
                {
                    var match = column.Slots.Where(x => x.Identifier == id).FirstOrDefault();

                    if (match != null)
                    {
                        section.RenameSlot(id, newName);
                        Serialize();
                    }
                }
            }
        }

        public void RenameSection(Guid id, string newName)
        {
            EnsureSections();

            var match = _Sections.Where(x => x.Identifier == id).FirstOrDefault();

            if (match != null)
            {
                match.Name = newName;
                Serialize();
            }
        }

        public ISlot CreateSlot(Guid sectionId,
            int column,
            int position,
            System.Type dataSource,
            System.Type renderer,
            IDictionary<string, string> options,
            string title,
            int X,
            int Y)
        {
            EnsureSections();

            var section = _Sections.Where(x => x.Identifier == sectionId).FirstOrDefault();

            if (section == null)
            {
                throw new Exception("Invalid section id");
            }

            var slot = new Slot();
            slot.Identifier = NewGuid();
            slot.SetTypes(dataSource, renderer);
            slot.Title = title;
            slot.X = X;
            slot.Y = Y;

            foreach (var option in options)
            {
                slot.Options[option.Key] =  option.Value;
            }

            section.AddSlot(slot,position,column);

            Serialize();

            return slot;
        }

        public ISection CreateSection(int columns, bool shared, string name)
        {
            EnsureSections();

            var section = new Section();
            section.Identifier = NewGuid();
            section.IsShared = shared;
            section.AddColumns(columns);
            section.Name = name;

            _Sections.Add(section);

            Serialize();

            return section;
        }

        private void Serialize()
        {
            EnsureSections();

            /* Start with private data */
            var data = new StringBuilder();
            var ser = new DataContractSerializer(typeof(Storage.PrivateData));
            using (XmlWriter xw = XmlWriter.Create(data))
            {
                var model = new Storage.PrivateData();
                model.PrivateSections = _Sections.Where(x => x.IsShared == false).ToList();
                model.DefaultSection = _DefaultSection;

                ser.WriteObject(xw, model);
                xw.Flush();
                UpdatePrivate(data.ToString());
            }

            /* Now do shared sections */

            foreach(var section in _Sections.Where(x => x.IsShared == true))
            {
                var sharedModel = new Storage.SharedData();
                sharedModel.SharedSection = section;

                data = new StringBuilder();
                ser = new DataContractSerializer(typeof(Storage.SharedData));

                using (XmlWriter xw = XmlWriter.Create(data))
                {
                    ser.WriteObject(xw, sharedModel);
                    xw.Flush();
                    UpdateShared(data.ToString(), section.Identifier);
                }
            }

        }

        public void LoadPrivate(string data)
        {
            EnsureSections();

            XmlDictionaryReader reader =
                XmlDictionaryReader.CreateTextReader(Encoding.Unicode.GetBytes(data), new XmlDictionaryReaderQuotas());
            var ser = new DataContractSerializer(typeof(Storage.PrivateData));

            var model = (Storage.PrivateData)ser.ReadObject(reader);

            this._Sections.AddRange(model.PrivateSections);
            this._DefaultSection = model.DefaultSection;
        }


        public void LoadShared(string data)
        {
            EnsureSections();

            XmlDictionaryReader reader =
                XmlDictionaryReader.CreateTextReader(Encoding.Unicode.GetBytes(data), new XmlDictionaryReaderQuotas());
            var ser = new DataContractSerializer(typeof(Storage.SharedData));

            var model = (Storage.SharedData)ser.ReadObject(reader);
            this._Sections.Add(model.SharedSection);
        }


        public void SetDefaultSection(Guid id)
        {
            _DefaultSection = id;
            Serialize();
        }

        public Guid GetDefaultSection()
        {
            return _DefaultSection;
        }

        private void EnsureSections()
        {
            if (_Sections == null)
            {
                _Sections = new List<Section>();
            }
        }


        private static Guid NewGuid()
        {
            Guid value = Guid.Empty;
            int hresult = CoCreateGuid(ref value);

            if (hresult != 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Error creating new Guid");
            }

            return value;
        }

        [DllImport("ole32.dll", SetLastError = true)]
        private static extern int CoCreateGuid(ref Guid pguid);
    }
}
