using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.DataSource;
using SnyderIS.sCore.Exi.Interfaces.Renderer;
using SnyderIS.sCore.Exi.Interfaces.Widget;

namespace SnyderIS.sCore.Exi.Interfaces.Canvas
{
    public interface ICanvas
    {
        IEnumerable<ISection> GetSections();
        
        void RemoveSection(Guid id);
        void RemoveSlot(Guid id);
        void MoveSlot(Guid id, int newPosition, int newColumn, int newX, int newY);

        void RenameSection(Guid id, string newName);
        void RenameSlot(Guid id, string newName);

        ISlot CreateSlot(Guid sectionId,
            int column,
            int position,
            System.Type dataSource,
            System.Type renderer,
            IDictionary<string,string> options,
            string title,
            int X,
            int Y);

        ISection CreateSection(int columns, 
            bool shared,
            string name);

        void SetDefaultSection(Guid id);
        Guid GetDefaultSection();
    }
}
