using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnyderIS.sCore.Exi.Kiosk.Modules.HeartBeat
{
    public class HeartBeatStatusCommand : BaseCommand
    {
        private HeartBeatModule _module;

        public HeartBeatStatusCommand(HeartBeatModule m, MainViewImpl mv)
            : base(mv)
        {
            _module = m;
        }

        public override void Run(string[] args)
        {
            this.WriteMessage(
                string.Format("Last heartbeat: {0} seconds ago", DateTime.Now.Subtract(_module.GetHeartBeat()).TotalSeconds));
        }
    }
}
