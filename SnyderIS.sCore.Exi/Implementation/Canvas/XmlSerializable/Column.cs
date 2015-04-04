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
    public class Column : IColumn
    {
        [DataMember(Name = "_Slots")]
        private List<Slot> _Slots;

        public IEnumerable<ISlot> Slots
        {
            get
            {

                if (_Slots == null)
                {
                    _Slots = new List<Slot>();
                }

                return _Slots;
            }
        }

        public void RemoveSlot(Guid id)
        {
            EnsureSlots();

            var match = _Slots.Where(x => x.Identifier == id).FirstOrDefault();

            if (match != null)
            {
                _Slots.Remove(match);
            }
        }


        public void RenameSlot(Guid id, string newName)
        {
            EnsureSlots();

            var match = _Slots.Where(x => x.Identifier == id).FirstOrDefault();

            if (match != null)
            {
                match.Title = newName;
            }
        }

        public void AddSlot(Slot slot, int position)
        {
            EnsureSlots();
            _Slots.Insert(position, slot);
        }

        private void EnsureSlots()
        {
            if (_Slots == null)
            {
                _Slots = new List<Slot>();
            }
        }

    }
}
