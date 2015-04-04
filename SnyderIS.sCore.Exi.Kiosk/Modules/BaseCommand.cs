using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnyderIS.sCore.Exi.Kiosk.Modules
{
    public abstract class BaseCommand : IKioskCommand
    {
        protected MainViewImpl _mainView;

        public BaseCommand(MainViewImpl main)
        {
            _mainView = main;
        }

        public abstract void Run(string[] args);

        public void WriteMessage(string message)
        {
            _mainView.ShellLog(message);
        }
    }
}
