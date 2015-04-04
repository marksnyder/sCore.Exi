
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

    public class GtkGraphics
    {
        private MainViewImpl _mainView;
        private Window _messageWindow;
        private Gtk.DrawingArea _messageDrawingArea;
        private Pango.Layout _messageLayout;
        private int _slideIndex = -100000;
        private Gdk.Pixbuf _logo;

        public  GtkGraphics(MainViewImpl mainview)
        {

            _mainView = mainview;

            int y = Configuration.MessageBoxYOverride.HasValue ? Configuration.MessageBoxYOverride.Value : _mainView.Screen.Height - 101;

            /* Setup message window */
            _messageWindow = new Window(WindowType.Popup);

            _messageWindow.Move(0, y);
            _messageWindow.Resize(_mainView.Screen.Width, 100);
            _messageWindow.Opacity = .7;
            _messageWindow.Show();
            _messageWindow.DoubleBuffered = false;
            _messageWindow.AppPaintable = true;

            Gdk.Color windowBGColor = new Gdk.Color();
            Gdk.Color.Parse("black", ref windowBGColor);
            _messageWindow.ModifyBg(StateType.Normal, windowBGColor);


            _messageLayout = new Pango.Layout(_messageWindow.PangoContext);
            //_messageLayout.Width = Pango.Units.FromPixels(Screen.Width * 2);

            _messageLayout.Wrap = Pango.WrapMode.Word;
            _messageLayout.Alignment = Pango.Alignment.Left;
            _messageLayout.FontDescription = new Pango.FontDescription() { Family = "Sans", Weight = Pango.Weight.Bold, Size = (int)(48 * Pango.Scale.PangoScale) };

            Gdk.Color labelColor = new Gdk.Color();
            Gdk.Color.Parse("yellow", ref labelColor);

            _messageDrawingArea = new Gtk.DrawingArea();
            _messageDrawingArea.SetSizeRequest(_mainView.Screen.Width, 100);
            _messageDrawingArea.ModifyBg(StateType.Normal, windowBGColor);
            _messageDrawingArea.ModifyFg(StateType.Normal, labelColor);

            _messageWindow.Add(_messageDrawingArea);
            _messageWindow.ShowAll();

        }

        public bool DrawMessage(string messageText)
        {

            if (messageText == string.Empty)
            {
                if (_messageWindow.Visible)
                {
                    _messageWindow.Hide();
                }
            }
            else
            {
                if (!_messageWindow.Visible)
                {
                    _messageWindow.Show();
                }

                _messageLayout.SetMarkup(messageText);

                _slideIndex = _slideIndex - Configuration.MessageSpeed;

                _messageDrawingArea.DoubleBuffered = true;
                _messageDrawingArea.AppPaintable = true;


                _messageDrawingArea.GdkWindow.DrawLayout(_messageDrawingArea.Style.TextGC(StateType.Normal),
                    _slideIndex, 5, _messageLayout);




                int height;
                int width;

                _messageLayout.GetPixelSize(out width, out height);

                if (_slideIndex < 0 - (width))
                {
                    _slideIndex = _mainView.Screen.Width;
                }

                _messageDrawingArea.GdkWindow.ClearArea(
                    _slideIndex + width + 1, 0,
                    Configuration.MessageSpeed + 1, height + 5);

                _messageDrawingArea.GdkWindow.DrawPixbuf(_messageDrawingArea.Style.TextGC(StateType.Normal),
                    _logo, 0, 0,
                    10, 5,
                    _logo.Width, _logo.Height,
                     Gdk.RgbDither.None, 0, 0);
            }

            return true;
        }

        public void LoadLogo(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream stream = httpWebReponse.GetResponseStream();
            _logo = new Gdk.Pixbuf(stream);
        }
    }
}
