using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ERPSolutions_EF.DAL;
using ERPSolutions_EF.Models;
using ERPSolutions_EF.ViewModels;

namespace ERPSolutions_EF.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var employee = Employee.GetEmployee(User.Identity.Name);
            if (!employee.HasRole((int)Enums.Roles.AUTHOR))
            {
                if (!employee.HasRole((int)Enums.Roles.ADMINISTRATOR))
                {
                    if (employee.HasRole((int)Enums.Roles.PERFORMER))
                        return RedirectToAction("Index", "perform");
                    else if (employee.HasRole((int)Enums.Roles.APPROVER))
                        return RedirectToAction("Index", "approve");

                    return View("Block");
                }
            }

            var model = MainForm(employee);
            model.PriorityId = (int)Enums.Priorities.B;

            return View(model);
        }

        public ActionResult AddTicket(InitiateViewModel model)
        {
            if (model.PriorityId == (int)Enums.Priorities.A)
            {
                if (model.PriorityDesc == null)
                    ModelState.AddModelError("PriorityDesc", "Не указано обоснование класса А");
            }

            if (!ModelState.IsValid)
            {
                var employee = Employee.GetEmployee(User.Identity.Name);
                var data = MainForm(employee);

                data.Solutions = model.Solutions ?? null;
                data.TechnicalTask = model.TechnicalTask ?? null;
                data.Desc = model.Desc ?? null;
                data.FullDesc = model.FullDesc ?? null;
                data.Instructions = model.Instructions ?? null;
                data.PriorityDesc = model.PriorityDesc ?? null;

                if (model.PriorityId != (int)Enums.Priorities.B)
                    data.PriorityId = model.PriorityId;

                if (model.TesterId != 1)
                    data.TesterId = model.TesterId;

                return View("Index", data);
            }

            using (var ctx = new SolutionsContext())
            {
                var ticket = new Ticket()
                {
                    AuthorId = model.AuthorId,
                    Desc = model.Desc,
                    FullDesc = model.FullDesc,
                    Instructions = model.Instructions,
                    OperationId = model.OperationId,
                    PriorityId = model.PriorityId,
                    ResourceId = model.ResourceId,
                    Solutions = model.Solutions,
                    TechnicalTask = model.TechnicalTask,
                    Created = DateTime.Now
                };

                if (ticket.ResourceId == (int)Enums.Resources.PRODUCTION)
                {
                    ticket.TesterId = model.TesterId;
                    ticket.StatusId = (int)Enums.Statuses.ON_APPROVE;
                }
                else if (ticket.PriorityId == (int)Enums.Priorities.A)
                    ticket.StatusId = (int)Enums.Statuses.ON_APPROVE;
                else if (ticket.PriorityId == (int)Enums.Priorities.C)
                    ticket.StatusId = (int)Enums.Statuses.HOLD;
                else
                    ticket.StatusId = (int)Enums.Statuses.ON_PERFORM;

                ctx.Tickets.Add(ticket);
                ctx.SaveChanges();

                Mail.Send(ticket);

                return RedirectToAction("RequestSent");
            }
        }

        public ActionResult Edit(int ticketId)
        {
            var ticket = Ticket.ById(ticketId);
            Ticket.PrepareTicketToView(ticket);

            return View(ticket);
        }

        public ActionResult Reapprove(Ticket ticket, string comment)
        {
            if (!ModelState.IsValid)
            {
                var fullDesc = ticket.FullDesc ?? null;
                var instructions = ticket.Instructions ?? null;
                var technicalTask = ticket.TechnicalTask ?? null;

                var model = Ticket.ById(ticket.Id);
                Ticket.PrepareTicketToView(model);

                if (fullDesc != null)
                    model.FullDesc = fullDesc;
                if (instructions != null)
                    model.Instructions = instructions;
                if (technicalTask != null)
                    model.TechnicalTask = technicalTask;

                return View("Edit", model);
            }

            Ticket.Reapprove(ticket, comment);
            if (comment != string.Empty)
            {
                var approveComments = Comment.ByType(ticket.Id, (int)Enums.CommentTypes.Approve);
                var lastCommentId = approveComments.OrderBy(x => x.Date).Last().Id;
                Comment.Update(lastCommentId, comment);
            }

            return RedirectToAction("RequestSent");
        }

        public ActionResult Close(int ticketId)
        {
            Ticket.Close(ticketId);
            return RedirectToAction("Index");
        }

        private static InitiateViewModel MainForm(Employee author)
        {
            using (var ctx = new SolutionsContext())
            {
                var troubleStatuses = new List<int>()
                {
                    (int)Enums.Statuses.NOT_APPROVED
                };

                var activeStatuses = new List<int>()
                {
                    (int)Enums.Statuses.ON_APPROVE,
                    (int)Enums.Statuses.ON_PERFORM,
                    (int)Enums.Statuses.HOLD
                };

                var model = new InitiateViewModel()
                {
                    AuthorId = author.Id,
                    AuthorName = ActiveDirectory.GetName(author.Account),
                    Operations = new List<Operation>(ctx.Operations),
                    Priorities = new List<Priority>(ctx.Priorities),
                    Resources = new List<Resource>(ctx.Resources),
                    Testers = Employee.ByRoleRepo((int)Enums.Roles.TESTER),
                    TroubleTickets = Ticket.AuthorRepo(troubleStatuses, author),
                    ActiveTickets = Ticket.AuthorRepo(activeStatuses, author),
                    ClosedTickets = Ticket.ClosedRepo(author, (int)Enums.Roles.AUTHOR)
                };

                return model;
            }
        }

        public ActionResult RequestSent()
        {
            return View();
        }
    }
}