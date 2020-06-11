using System.Collections.Generic;
using System.Web.Mvc;
using ERPSolutions_EF.Models;
using ERPSolutions_EF.ViewModels;

namespace ERPSolutions_EF.Controllers
{
    public class ApproveController : Controller
    {
        public ActionResult Index()
        {
            var employee = Employee.GetEmployee(User.Identity.Name);
            if (employee != null)
            {
                if (!employee.HasRole((int)Enums.Roles.APPROVER) &&
                    !employee.HasRole((int)Enums.Roles.ADMINISTRATOR))
                    return RedirectToAction("Index", "Home");
                else
                {
                    var model = new ApproveViewModel()
                    {
                        Approver = employee,
                        ApprovalTickets = ApproveTickets(employee),
                        PerformingTickets = PerformTickets(employee),
                        ClosedTickets = Ticket.ClosedRepo(employee, (int)Enums.Roles.APPROVER)
                    };

                    return View(model);
                }
            }
            else
                return RedirectToAction("Index", "Home");
        }

        public static List<Ticket> ApproveTickets(Employee approver)
        {
            if (approver.HasRole((int)Enums.Roles.ADMINISTRATOR))
                return Ticket.ByStatusRepo((int)Enums.Statuses.ON_APPROVE);
            else
            {
                return Ticket.ByEmployeeRepo(
                    (int)Enums.Statuses.ON_APPROVE,
                    (int)Enums.Roles.APPROVER,
                    approver);
            }
        }

        public static List<Ticket> PerformTickets(Employee approver)
        {
            if (approver.HasRole((int)Enums.Roles.ADMINISTRATOR))
                return Ticket.ByStatusRepo((int)Enums.Statuses.ON_PERFORM);
            else
            {
                return Ticket.ByEmployeeRepo(
                    (int)Enums.Statuses.ON_PERFORM,
                    (int)Enums.Roles.APPROVER,
                    approver);
            }
        }

        public ActionResult Process(int ticketId, string approve, string disapprove, string comment)
        {
            var approver = Employee.GetEmployee(User.Identity.Name);

            if (approve != null)
                Ticket.Approve(ticketId, approver.Id, comment);
            else if (disapprove != null)
                Ticket.Reject(ticketId, approver.Id, comment);

            if (comment != string.Empty)
                Comment.Save(ticketId, approver.Id, comment, (int)Enums.CommentTypes.Approve);

            return RedirectToAction("Index");
        }

        public ActionResult Assign(int ticketId, int performerId)
        {
            Ticket.Assign(ticketId, performerId, null);
            return RedirectToAction("Index");
        }

        public ActionResult HistoryTicketList(int ticketId)
        {
            var history = Ticket.History(Ticket.ById(ticketId).Solutions, ticketId);
            return PartialView("_SolutionHistoryList", history);
        }
    }
}