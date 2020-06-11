using System.Collections.Generic;
using System.Web.Mvc;
using ERPSolutions_EF.Models;
using ERPSolutions_EF.ViewModels;

namespace ERPSolutions_EF.Controllers
{
    public class PerformController : Controller
    {
        public ActionResult Index()
        {
            var employee = Employee.GetEmployee(User.Identity.Name);
            if (employee != null)
            {
                if (!employee.HasRole((int)Enums.Roles.PERFORMER) &&
                    !employee.HasRole((int)Enums.Roles.ADMINISTRATOR))
                    return RedirectToAction("Index", "Home");
                else
                {
                    var model = new PerformViewModel()
                    {
                        Performer = employee,
                        HoldTickets = HoldTickets(employee),
                        PerformTickets = PerformTickets(employee),
                        ClosedTickets = Ticket.ClosedRepo(employee, (int)Enums.Roles.PERFORMER)
                    };

                    Session["performerId"] = employee.Id;
                    return View(model);
                }
            }
            else
                return RedirectToAction("Index", "Home");
        }

        public static List<Ticket> HoldTickets(Employee performer)
        {
            if (performer.HasRole((int)Enums.Roles.ADMINISTRATOR))
                return Ticket.ByStatusRepo((int)Enums.Statuses.HOLD);
            else
            {
                return Ticket.ByEmployeeRepo(
                    (int)Enums.Statuses.HOLD,
                    (int)Enums.Roles.PERFORMER,
                    performer);
            }
        }

        public static List<Ticket> PerformTickets(Employee performer)
        {
            if (performer.HasRole((int)Enums.Roles.ADMINISTRATOR))
                return Ticket.ByStatusRepo((int)Enums.Statuses.ON_PERFORM);
            else
            {
                return Ticket.ByEmployeeRepo(
                    (int)Enums.Statuses.ON_PERFORM,
                    (int)Enums.Roles.PERFORMER,
                    performer);
            }
        }

        public ActionResult Process(int ticketId, string assign, string perform,
                                    string hold, string decline, string comment)
        {
            var performer = Employee.GetEmployee(User.Identity.Name);

            if (assign != null)
                Ticket.Assign(ticketId, performer.Id, comment);
            else if (perform != null)
                Ticket.Perform(ticketId, performer.Id, comment);
            else if (hold != null)
                Ticket.Hold(ticketId, performer.Id, comment);
            else if (decline != null)
                Ticket.Decline(ticketId, performer.Id, comment);

            if (comment != string.Empty)
                Comment.Save(ticketId, performer.Id, comment, (int)Enums.CommentTypes.Perform);

            return RedirectToAction("Index");
        }
    }
}