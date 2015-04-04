using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnyderIS.sCore.Exi.Kiosk.Modules.HeartBeat
{
    public class HeartBeatCommand : BaseCommand
    {
        private HeartBeatModule _module;

        public HeartBeatCommand(HeartBeatModule m, MainViewImpl mv)
            : base(mv)
        {
            _module = m;
        }

        public override void Run(string[] args)
        {
            _module.SetHeartBeat();
        }
    }
}
