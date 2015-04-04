using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.Widget;
using SnyderIS.sCore.Exi.Interfaces.Renderer;
using SnyderIS.sCore.Exi.Interfaces.DataSource;


namespace SnyderIS.sCore.Exi.Implementation.Widget.FluentWizard
{
    public class WidgetGroupBuilder
    {
        private WizardBuilder _Parent = null;
        private Func<bool> _Conditional = null;
        private string _Name = string.Empty;
        private IList<WidgetBuilder> _Builders = new List<WidgetBuilder>();
        private string _Description = string.Empty;

        public WidgetGroupBuilder(WizardBuilder parent)
        {
            _Parent = parent;
        }

        public WidgetGroupBuilder If(Func<bool> conditional)
        {
            _Conditional = conditional;
            return this;
        }

        public WidgetGroupBuilder Named(string name)
        {
            _Name = name;
            return this;
        }

        public WidgetGroupBuilder WithDescription(string description)
        {
            _Description = description;
            return this;
        }

        public WidgetBuilder BeginWidget()
        {
            var builder = new WidgetBuilder(this);
            _Builders.Add(builder);
            return builder;
        }

        public IWidgetGroup Build()
        {
            if (_Conditional != null)
            {
                if (!_Conditional.Invoke())
                {
                    return null;
                }
            }

            var entity = new Entities.WidgetGroup();
            var widgets = new List<IWidget>();

            foreach (var b in _Builders)
            {
                var e = b.Build();

                if (e != null)
                {
                    widgets.Add(e);
                }
            }

            entity.Widgets = widgets;

            entity.Name = _Name;
            entity.Description = _Description;
            return entity;
        }

        
    }
}
