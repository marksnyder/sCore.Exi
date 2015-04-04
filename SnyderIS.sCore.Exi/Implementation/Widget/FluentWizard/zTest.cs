using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnyderIS.sCore.Exi.Implementation.Widget.FluentWizard
{
    internal class zTest : WizardBuilder
    {
        public zTest()
        {

            this
                .BeginGroup()
                .If(DontDoIt)
                    .Named("TAT - 30 Day")
                    .WithDescription("30 days worth of turn around time information on a specific test.")
                /* 30 day turn aroudn time widget */
                    .BeginWidget()
                        .If(DoIt)
                        .SetDataSource<string>()
                        .IncludeRenderer<string>()
                            .Named("Simple Volume Graph")
                            .WithDescription("A simple graph displaying volume for a specific test")
                            .EndRenderer()
                        .IncludeRenderer<string>()
                            .Named("Simple Volume Graph")
                            .WithDescription("A simple graph displaying volume for a specific test")
                            .EndRenderer()
                        .IncludeRenderer<string>()
                            .Named("Simple Volume Graph")
                            .WithDescription("A simple graph displaying volume for a specific test")
                            .EndRenderer()
                            .BeginOption()
                                .WithKey("CLIENT")
                                .WithLabel("Client Code")
                                .WithDefaultValue("2")
                                .Required()
                              .EndOption()
                    .EndWidget()
                /* 7 Day turn around time widget */
                    .BeginWidget()
                        .If(DoIt)
                        .GenerateTatOptions()
                    .EndWidget();
                        

                      
                    
                        
                        
                

        }

        private bool DontDoIt()
        {
            return false;
        }

        private bool DoIt()
        {
            return true;
        }

    }

    public static class TestHelper
    {
        public static WidgetBuilder GenerateTatOptions(this WidgetBuilder builder)
        {
            return builder
                    .BeginOption()
                        .WithKey("CLIENT")
                        .WithLabel("Client Code")
                        .WithDefaultValue("2")
                        .Required()
                        .EndOption();

                
        }
    }
}
