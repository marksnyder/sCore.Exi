using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnyderIS.sCore.Exi.Kiosk.Modules.MessageScroller
{
    public class MessageChangeCommand : BaseCommand
    {

        private MessageScrollerModule _module;

        public MessageChangeCommand(MessageScrollerModule m, MainViewImpl mv)
            : base(mv)
        {
            _module = m;
        }


        public override void Run(string[] args)
        {
            var message = string.Empty;

            foreach (var m in args)
            {
                message = string.Concat(message, " ", m);
            }

            _module.SetMessage(message.Trim());

           this.WriteMessage(
                string.Format("Message changed to {0}",message));
        }
    }
}
