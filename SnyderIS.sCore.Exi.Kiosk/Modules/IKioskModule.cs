using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnyderIS.sCore.Exi.Kiosk.Modules
{
    public interface IKioskModule
    {
        void Initialize(MainViewImpl mainView);
    }
}
