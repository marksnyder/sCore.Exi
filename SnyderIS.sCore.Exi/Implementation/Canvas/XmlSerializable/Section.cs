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
    public class Section : ISection
    {
        [DataMember(Name = "_Columns")]
        private List<Column> _Columns;

        [DataMember(Name = "Identifier")]
        public Guid Identifier { get; set; }

        [DataMember(Name = "Name")]
        public string Name { get; set; }

        [DataMember(Name = "IsShared")]
        public bool IsShared { get; set; }

        public IEnumerable<IColumn> Columns
        {
            get
            {
                return _Columns;
            }
        }

        public void RemoveSlot(Guid id)
        {
            EnsureColumns();

            foreach (var column in _Columns)
            {
                var match = column.Slots.Where(x => x.Identifier == id).FirstOrDefault();

                if (match != null)
                {
                    column.RemoveSlot(id);
                }
            }

        }

        public void RenameSlot(Guid id, string newName)
        {
            EnsureColumns();

            foreach (var column in _Columns)
            {
                var match = column.Slots.Where(x => x.Identifier == id).FirstOrDefault();

                if (match != null)
                {
                    column.RenameSlot(id,newName);
                }
            }

        }

        public void AddSlot(Slot slot, int position, int column)
        {
            EnsureColumns();
            _Columns[column].AddSlot(slot, position);
        }

        public void MoveSlot(Guid id, int newPosition, int newColumn, int newX, int newY)
        {

            foreach (var column in _Columns)
            {
                var match = (Slot)column.Slots.Where(x => x.Identifier == id).FirstOrDefault();

                if (match != null)
                {
                    column.RemoveSlot(id);
                    match.X = newX;
                    match.Y = newY;
                    _Columns[newColumn].AddSlot(match, newPosition);
                }
            }
        }

        public void AddColumns(int count)
        {
            EnsureColumns();

            for(int i = 1; i <= count; i++)
            {
                _Columns.Add(new Column());
            }
        }

        private void EnsureColumns()
        {
            if (_Columns == null)
            {
                _Columns = new List<Column>();
            }
        }

    }
}
