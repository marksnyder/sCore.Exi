using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.Widget;
using SnyderIS.sCore.Exi.Interfaces.Renderer;
using SnyderIS.sCore.Exi.Interfaces.DataSource;

namespace SnyderIS.sCore.Exi.Implementation.Widget.FluentWizard
{
    public class WidgetBuilder
    {
        private WidgetGroupBuilder _Parent = null;
        private Func<bool> _Conditional = null;
        private List<RendererBuilder> _RendererBuilders = new List<RendererBuilder>();
        private System.Type _DataSource = null;
        private IList<OptionBuilder> _OptionBuilders = new List<OptionBuilder>();
        private string _Name = string.Empty;
        private string _Description = string.Empty;

        public WidgetBuilder(WidgetGroupBuilder parent)
        {
            _Parent = parent;
        }

        public WidgetBuilder If(Func<bool> conditional)
        {
            _Conditional = conditional;
            return this;
        }

        public WidgetBuilder SetDataSource<TTT>()
        {
            _DataSource = typeof(TTT);
            return this;
        }

        public RendererBuilder IncludeRenderer<TTT>()
        {
            var builder = new RendererBuilder(this, typeof(TTT));
            _RendererBuilders.Add(builder);
            return builder;
        }

        public WidgetBuilder Named(string name)
        {
            _Name = name;
            return this;
        }

        public WidgetBuilder WithDescription(string description)
        {
            _Description = description;
            return this;
        }

        public OptionBuilder BeginOption()
        {
            var builder = new OptionBuilder(this);
            _OptionBuilders.Add(builder);
            return builder;
        }

        public WidgetGroupBuilder EndWidget()
        {
            return _Parent;
        }

        public IWidget Build()
        {
            if (_Conditional != null)
            {
                if (!_Conditional.Invoke())
                {
                    return null;
                }
            }

            var entity = new Entities.Widget();
            entity.Name = _Name;
            entity.Description = _Description;
            entity.DataSourceTypeName = this._DataSource.AssemblyQualifiedName;

            var renders = new List<IWidgetRenderer>();

            foreach (var rBuilder in _RendererBuilders)
            {
                var r = rBuilder.BuildRenderer();

                if (r != null)
                {
                    renders.Add(r);
                }
            }

            entity.RendererOptions = renders;

            var options = new List<IWidgetOption>();

            foreach (var ogBuilder in _OptionBuilders)
            {
                var o = ogBuilder.Build();

                if (o != null)
                {
                    options.Add(o);
                }
            }

            entity.Options = options;


            return entity;
        }
    }
}
