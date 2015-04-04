using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using SnyderIS.sCore.Exi.Interfaces.Canvas;
using SnyderIS.sCore.Exi.Interfaces.DataSource;
using SnyderIS.sCore.Exi.Interfaces.Renderer;
using SnyderIS.sCore.Exi.Mvc;
using System.IO;

namespace SnyderIS.sCore.Exi.Mvc.Renderers
{
    public abstract class MvcRenderer<T> : IRenderer<T>
    {

        public abstract string Name { get; }

        public abstract string Description { get; }

        public abstract string RenderHtml(IDataSourceResult<T> data);

        string IRenderer.RenderHtml(IDataSourceResult data)
        {
            return RenderHtml((IDataSourceResult<T>)data);
        }

        protected Controller Controller
        {
            get
            {
                return (Controller)System.Web.HttpContext.Current.Items["sCoreExiRenderingController"];
            }
        }

        protected string RenderPartialView(string viewName, object model)
        {

            if (string.IsNullOrEmpty(viewName))
            {
                viewName = this.Controller.ControllerContext.RouteData.GetRequiredString("action");
            }

            this.Controller.ViewData.Model = model;

            using (StringWriter stringWriter = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.Controller.ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(this.Controller.ControllerContext, viewResult.View, this.Controller.ViewData, this.Controller.TempData, stringWriter);
                viewResult.View.Render(viewContext, stringWriter);
                return stringWriter.GetStringBuilder().ToString();
            }
        }


    }
}
