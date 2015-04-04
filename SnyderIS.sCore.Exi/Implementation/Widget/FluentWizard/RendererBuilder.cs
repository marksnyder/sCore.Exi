using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.Widget;
using SnyderIS.sCore.Exi.Interfaces.Renderer;
using SnyderIS.sCore.Exi.Interfaces.DataSource;

namespace SnyderIS.sCore.Exi.Implementation.Widget.FluentWizard
{
    public class RendererBuilder
    {
        private WidgetBuilder _Parent = null;
        private Func<bool> _Conditional = null;
        private string _Name = string.Empty;
        private string _Description = string.Empty;
        private System.Type _RendererType = null;

        public RendererBuilder(WidgetBuilder parent, System.Type rendererType)
        {
            _Parent = parent;
            _RendererType = rendererType;
        }

        public RendererBuilder If(Func<bool> conditional)
        {
            _Conditional = conditional;
            return this;
        }

        public RendererBuilder Named(string name)
        {
            _Name = name;
            return this;
        }

        public RendererBuilder WithDescription(string description)
        {
            _Description = description;
            return this;
        }

        public WidgetBuilder EndRenderer()
        {
            return _Parent;
        }

        public IWidgetRenderer BuildRenderer()
        {
            if (_Conditional != null)
            {
                if (!_Conditional.Invoke())
                {
                    return null;
                }
            }

            var entity = new Entities.WidgetRenderer();
            entity.Description = _Description;
            entity.Name = _Name;
            entity.TypeName = _RendererType.AssemblyQualifiedName;

            return entity;
        }
    }
}
