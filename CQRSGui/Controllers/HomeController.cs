using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using readmodel = ReadModel.InMemoryDatabase;
using bus = EventBus;

namespace CQRSGui.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            ViewData.Model = readmodel.InventoryItems;

            return View();
        }
        public ActionResult Details(Guid id)
        {
            ViewData.Model = readmodel.InventoryItemDetails[id];
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(string name)
        {
            var command = Commands.Command.NewCreateInventoryItem(Guid.NewGuid(), name);
            CommandHandler.Handle(command);

            return RedirectToAction("Index");
        }

        public ActionResult ChangeName(Guid id)
        {
            ViewData.Model = readmodel.InventoryItemDetails[id];
            return View();
        }

        [HttpPost]
        public ActionResult ChangeName(Guid id, string name)
        {
            var command = Commands.Command.NewRenameInventoryItem(id, name);
            CommandHandler.Handle(command);

            return RedirectToAction("Index");
        }

        public ActionResult Deactivate(Guid id)
        {
            var command = Commands.Command.NewDeactivateInventoryItem(id);
            CommandHandler.Handle(command);

            return RedirectToAction("Index");
        }

        public ActionResult CheckIn(Guid id)
        {
            ViewData.Model = readmodel.InventoryItemDetails[id];
            return View();
        }

        [HttpPost]
        public ActionResult CheckIn(Guid id, int number)
        {
            var command = Commands.Command.NewCheckInItemsToInventory(id, number);
            CommandHandler.Handle(command);

            return RedirectToAction("Index");
        }

        public ActionResult Remove(Guid id)
        {
            ViewData.Model = readmodel.InventoryItemDetails[id];
            return View();
        }

        [HttpPost]
        public ActionResult Remove(Guid id, int number)
        {
            var command = Commands.Command.NewRemoveItemsFromInventory(id, number);
            CommandHandler.Handle(command);

            return RedirectToAction("Index");
        }

    }
}
