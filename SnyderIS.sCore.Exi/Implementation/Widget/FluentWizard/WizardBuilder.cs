using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnyderIS.sCore.Exi.Interfaces.Widget;
using SnyderIS.sCore.Exi.Interfaces.Renderer;
using SnyderIS.sCore.Exi.Interfaces.DataSource;
using System.Linq.Expressions;

namespace SnyderIS.sCore.Exi.Implementation.Widget.FluentWizard
{
    public class WizardBuilder : IWizardBuilder
    {
        private IList<WidgetGroupBuilder> _GroupBuilders = new List<WidgetGroupBuilder>();

        IEnumerable<IWidgetGroup> IWizardBuilder.BuildGroups()
        {
            var groups = new List<IWidgetGroup>();

            foreach (var groupBuilder in _GroupBuilders)
            {
                var group = groupBuilder.Build();

                if (group != null)
                {
                    groups.Add(group);
                }
            }

            return groups;
        }

        public WidgetGroupBuilder BeginGroup()
        {
            var builder = new WidgetGroupBuilder(this);
            _GroupBuilders.Add(builder);
            return builder;
        }

    }
}
