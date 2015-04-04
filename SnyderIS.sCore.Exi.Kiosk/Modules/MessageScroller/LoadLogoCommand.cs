using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnyderIS.sCore.Exi.Kiosk.Modules.MessageScroller
{
    public class LoadLogoCommand : BaseCommand
    {

        private MessageScrollerModule _module;

        public LoadLogoCommand(MessageScrollerModule m, MainViewImpl mv)
            : base(mv)
        {
            _module = m;
        }

        public override void Run(string[] args)
        {
            if (args.Length > 0)
            {
                _module.LoadLogo(args.First());
            }
        }
    }
}
