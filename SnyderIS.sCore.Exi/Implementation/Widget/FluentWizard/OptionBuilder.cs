using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.Widget;
using SnyderIS.sCore.Exi.Interfaces.Renderer;
using SnyderIS.sCore.Exi.Interfaces.DataSource;

namespace SnyderIS.sCore.Exi.Implementation.Widget.FluentWizard
{
    public class OptionBuilder
    {
        private WidgetBuilder _Parent = null;
        private Func<bool> _Conditional = null;
        private string _Key = string.Empty;
        private string _Label = string.Empty;
        private bool _Required = false;
        private bool _Hidden = false;
        private object _DefaultValue = null;
        private IEnumerable<KeyValuePair<string, string>> _SelectList { get; set; }

        public OptionBuilder(WidgetBuilder parent)
        {
            _Parent = parent;
        }

        public OptionBuilder If(Func<bool> conditional)
        {
            _Conditional = conditional;
            return this;
        }

        public OptionBuilder Required()
        {
            _Required = true;
            return this;
        }

        public OptionBuilder WithKey(string key)
        {
            _Key = key;
            return this;
        }

        public OptionBuilder WithLabel(string label)
        {
            _Label = label;
            return this;
        }

        public OptionBuilder WithDefaultValue(object val)
        {
            _DefaultValue = val;
            return this;
        }

        public OptionBuilder WithSelectList(IEnumerable<KeyValuePair<string, string>> list)
        {
            _SelectList = list;
            return this;
        }

        public OptionBuilder Hidden()
        {
            _Hidden = true;
            return this;
        }

        public WidgetBuilder EndOption()
        {
            return _Parent;
        }


        public IWidgetOption Build()
        {
            if (_Conditional != null)
            {
                if (!_Conditional.Invoke())
                {
                    return null;
                }
            }

            var e = new Entities.WidgetOption();
            e.DefaultValue = this._DefaultValue;
            e.Label = this._Label;
            e.Required = this._Required;
            e.SelectList = null;
            e.Key = this._Key;
            e.Hidden = this._Hidden;
            e.SelectList = this._SelectList;
            return e;
        }
    }
}
