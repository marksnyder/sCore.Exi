using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using SnyderIS.sCore.Exi.Interfaces.Canvas;
using SnyderIS.sCore.Exi.Interfaces.DataSource;
using SnyderIS.sCore.Exi.Interfaces.Renderer;
using SnyderIS.sCore.Exi.Interfaces.Widget;
using SnyderIS.sCore.Exi.Mvc;

namespace SnyderIS.sCore.Exi.Mvc.Controllers
{
    public abstract class Canvas : Controller
    {
        public abstract ICanvas GetCanvas();
        public abstract IWizardBuilder GetWizardBuilder();

        public ActionResult ExiGetCanvasJson()
        {
            var result = new Json.Canvas();
            result.Sections = new List<Json.Section>();

            foreach (var section in GetCanvas().GetSections())
            {
                var jSection = new Json.Section();
                jSection.Name = section.Name;
                jSection.Identifier = section.Identifier;
                jSection.IsShared = section.IsShared;
                jSection.Columns = new List<Json.Column>();
                result.Sections.Add(jSection);

                foreach (var column in section.Columns)
                {
                    var jColumn = new Json.Column();
                    jColumn.Slots = new List<Mvc.Json.Slot>();

                    jSection.Columns.Add(jColumn);

                    foreach (var slot in column.Slots)
                    {
                        var jSlot = new Json.Slot();
                        jSlot.Identifier = slot.Identifier;
                        jSlot.Title = slot.Title;
                        jSlot.X = slot.X;
                        jSlot.Y = slot.Y;
                        jColumn.Slots.Add(jSlot);
                    }
                }
            }

            result.DefaultSection = GetCanvas().GetDefaultSection();

            return Json(result, JsonRequestBehavior.AllowGet);  
        }

        public ActionResult ExiSetDefaultSection(Guid id)
        {
            GetCanvas().SetDefaultSection(id);
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExiCreateSlot(Guid sectionId,
            int column,
            int position,
            string dataSourceTypeName,
            string rendererTypeName,
            string options,
            string title,
            int X,
            int Y)
        {
            var dataSourceType = System.Type.GetType(dataSourceTypeName);
            var rendererType = System.Type.GetType(rendererTypeName);


            var optionData = new Dictionary<string,string>();

            if (options != null && options != string.Empty)
            {
                foreach (var opt in options.Split(','))
                {
                    var key = opt.Split(':')[0];
                    var val = opt.Split(':')[1];
                    optionData[key] = val;
                }
            }

            var dataSource = (IDataSource)DependencyResolver.Current.GetService(dataSourceType);

            var newSlot = GetCanvas().CreateSlot(
                sectionId,
                column,
                position,
                dataSourceType,
                rendererType,
                optionData,
                dataSource.DefaultTitle(optionData), X, Y);

            var result = new Json.Slot();
            result.Title = newSlot.Title;
            result.Identifier = newSlot.Identifier;


            OnStateChange();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExiRenameSlot(Guid id, string newName)
        {
            GetCanvas().RenameSlot(id, newName);
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExiMoveSlot(Guid id, int newPosition, int newColumn, int newX, int newY)
        {
            GetCanvas().MoveSlot(id, newPosition, newColumn, newX, newY);

            OnStateChange();

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExiRemoveSection(Guid id)
        {
            GetCanvas().RemoveSection(id);
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExiRenameSection(Guid id, string newName)
        {
            GetCanvas().RenameSection(id,newName);
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExiRemoveSlot(Guid id)
        {
            GetCanvas().RemoveSlot(id);

            OnStateChange();

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExiCreateSection(int columns,bool isShared, string name)
        {
            GetCanvas().CreateSection(columns, isShared,name);
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExiRenderSlot(Guid id)
        {

            OnCheckIn();

            System.Web.HttpContext.Current.Items["sCoreExiRenderingController"] = this;

            ISlot selectedSlot = null;
                
            /* TODO - optimize */
            foreach (var section in GetCanvas().GetSections())
            {
                foreach (var column in section.Columns)
                {
                    foreach (var slot in column.Slots)
                    {
                        if (slot.Identifier == id)
                        {
                            selectedSlot = slot;
                        }
                    }
                }
            }

            selectedSlot.Options.Add(new KeyValuePair<string,string>("slotid",selectedSlot.Identifier.ToString()));

            var dataSource = (IDataSource)DependencyResolver.Current.GetService(selectedSlot.DataSource);
            var renderer = (IRenderer)DependencyResolver.Current.GetService(selectedSlot.Renderer); ;

            var data = dataSource.GetResult(selectedSlot.Options);
            var content = renderer.RenderHtml(data);

            return Content(content);
        }

        public ActionResult ExiRenderPreview(string dataSourceTypeName,
            string rendererTypeName,
            string options)
        {
            try
            {
                this.HttpContext.Items["sCoreExiRenderingController"] = this;

                var optionData = new Dictionary<string, string>();

                if (options != null && options != string.Empty)
                {
                    foreach (var opt in options.Split(','))
                    {
                        var key = opt.Split(':')[0];
                        var val = opt.Split(':')[1];
                        optionData[key] = val;
                    }
                }

                var dataSourceType = System.Type.GetType(dataSourceTypeName);
                var rendererType = System.Type.GetType(rendererTypeName);
                var dataSource = (IDataSource)DependencyResolver.Current.GetService(dataSourceType); ;
                var renderer = (IRenderer)DependencyResolver.Current.GetService(rendererType); ;
                var data = dataSource.GetResult(optionData);
                var content = renderer.RenderHtml(data);
                return Content(content);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

        }

        public ActionResult ExiGetWizardDefinition()
        {
            var data = GetWizardBuilder().BuildGroups();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExiGetWizardTemplate()
        {
            var data = Properties.Resources.Wizard;
            return Content(data);
        }

        protected virtual void OnCheckIn()
        {
        }

        protected virtual void OnStateChange()
        {
        }


    }
}
