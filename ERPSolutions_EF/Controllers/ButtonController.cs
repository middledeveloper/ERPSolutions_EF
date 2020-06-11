using System.Web.Mvc;
using ERPSolutions_EF.Models;

namespace ERPSolutions_EF.Controllers
{
    public class ButtonController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Initiate()
        {
            return PartialView("Initiate");
        }

        public ActionResult Approve()
        {
            var employee = Employee.GetEmployee(User.Identity.Name);
            if (employee.HasRole((int)Enums.Roles.APPROVER) ||
                employee.HasRole((int)Enums.Roles.ADMINISTRATOR))
            {
                return PartialView("Approve");
            }

            return new EmptyResult();
        }

        public ActionResult Perform()
        {
            var employee = Employee.GetEmployee(User.Identity.Name);
            if (employee.HasRole((int)Enums.Roles.PERFORMER) ||
                employee.HasRole((int)Enums.Roles.ADMINISTRATOR))
            {
                return PartialView("Perform");
            }

            return new EmptyResult();
        }

        public ActionResult Report()
        {
            return PartialView("Report");
        }

        public ActionResult Settings()
        {
            var employee = Employee.GetEmployee(User.Identity.Name);
            if (employee.HasRole((int)Enums.Roles.ADMINISTRATOR))
            {
                return PartialView("Settings");
            }

            return new EmptyResult();
        }
    }
}