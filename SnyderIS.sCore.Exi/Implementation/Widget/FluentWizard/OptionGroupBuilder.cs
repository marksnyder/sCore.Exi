using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.Widget;
using SnyderIS.sCore.Exi.Interfaces.Renderer;
using SnyderIS.sCore.Exi.Interfaces.DataSource;


namespace SnyderIS.sCore.Exi.Implementation.Widget.FluentWizard
{
    public class OptionGroupBuilder
    {
        private WidgetBuilder _Parent = null;
        private Func<bool> _Conditional = null;

        private IList<WidgetGroupBuilder> _GroupBuilders = new List<WidgetGroupBuilder>();

        public OptionGroupBuilder(WidgetBuilder parent)
        {
            _Parent = parent;
        }

        public OptionGroupBuilder If(Func<bool> conditional)
        {
            _Conditional = conditional;
            return this;
        }

        public OptionBuilder BeginOptions()
        {
            return null;
        }

        public WidgetBuilder EndOptionGroup()
        {
            return null;
        }

        public IWidgetOptionGroup Build()
        {
            if (_Conditional != null)
            {
                if (!_Conditional.Invoke())
                {
                    return null;
                }
            }

            return null;
        }

    }
}
