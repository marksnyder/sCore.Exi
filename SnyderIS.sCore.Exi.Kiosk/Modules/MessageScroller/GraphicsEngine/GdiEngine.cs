

namespace SnyderIS.sCore.Exi.Kiosk.Modules.MessageScroller.GraphicsEngine
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Gtk;
    using MenuItemImpl = Gtk.MenuItem;
    using SnyderIS.sCore.Exi.Cef;
    using Xilium.CefGlue;
    using System.Net;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Drawing.Drawing2D;

    public class GdiEngine
    {
        private Window _messageWindow;
        private MainViewImpl _mainView;
        private System.Drawing.Bitmap _logo;
        private Image _image;
        private int _slideIndex = -100000;

        public GdiEngine(MainViewImpl mainview)
        {

            _mainView = mainview;
            int y = Configuration.MessageBoxYOverride.HasValue ? Configuration.MessageBoxYOverride.Value : _mainView.Screen.Height - 101;

            /* Setup message window */
            _messageWindow = new Window(WindowType.Popup);

            _messageWindow.Move(0, y);
            _messageWindow.Resize(_mainView.Screen.Width, 100);
            _messageWindow.Opacity = .7;
            _messageWindow.Show();

            _image = new Image();
            _messageWindow.Add(_image);
            _messageWindow.ShowAll();

            Gdk.Color windowBGColor = new Gdk.Color();
            Gdk.Color.Parse("black", ref windowBGColor);
            _messageWindow.ModifyBg(StateType.Normal, windowBGColor);


        }

        public bool DrawMessage(string messageText)
        {

            _slideIndex = _slideIndex - Configuration.MessageSpeed;

            var bmp = new System.Drawing.Bitmap(_mainView.Screen.Width, 100);
            var g = System.Drawing.Graphics.FromImage(bmp);

            var stringFont = new System.Drawing.Font("Arial", 48);
            var stringSize = g.MeasureString(messageText, stringFont);


            if (_slideIndex < 0 - (stringSize.Width))
            {
                _slideIndex = _mainView.Screen.Width;
            }

            g.DrawString(messageText, stringFont,
                new System.Drawing.SolidBrush(System.Drawing.Color.Yellow),
                _slideIndex, 10);


            if (_logo != null)
            {



                g.DrawImage(_logo, 10, 10);
            }

            var str = new System.IO.MemoryStream();
            bmp.Save(str, System.Drawing.Imaging.ImageFormat.Png);
            str.Seek(0, 0);


            var pb = new Gdk.Pixbuf(str);
            _image.Pixbuf = pb;

            return true;
        }

        public void LoadLogo(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream stream = httpWebReponse.GetResponseStream();
            _logo = new System.Drawing.Bitmap(stream);
        }
    }
}
