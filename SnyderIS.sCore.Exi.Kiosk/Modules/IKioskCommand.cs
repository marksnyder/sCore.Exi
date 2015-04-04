using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnyderIS.sCore.Exi.Kiosk.Modules
{
    public interface IKioskCommand
    {
        void Run(string[] args);
        void WriteMessage(string message);
    }
}
