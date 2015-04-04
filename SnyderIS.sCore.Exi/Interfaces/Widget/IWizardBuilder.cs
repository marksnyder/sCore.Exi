using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnyderIS.sCore.Exi.Interfaces.Widget
{
    public interface IWizardBuilder
    {
        IEnumerable<IWidgetGroup> BuildGroups();
    }
}
