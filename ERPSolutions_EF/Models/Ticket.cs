using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using ERPSolutions_EF.DAL;

namespace ERPSolutions_EF.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public Resource Resource { get; set; }
        public int ResourceId { get; set; }
        public Employee Author { get; set; }
        public int AuthorId { get; set; }
        public DateTime Created { get; set; }
        public Operation Operation { get; set; }
        public int OperationId { get; set; }
        [Required(ErrorMessage = "Укажите номер")]
        public string Solutions { get; set; }
        public string TechnicalTask { get; set; }
        [Required(ErrorMessage = "Укажите краткое описание")]
        public string Desc { get; set; }
        [AllowHtml]
        public string FullDesc { get; set; }
        [AllowHtml]
        public string Instructions { get; set; }
        public Priority Priority { get; set; }
        public int PriorityId { get; set; }
        public string PriorityDesc { get; set; }
        public Employee Tester { get; set; }
        public int? TesterId { get; set; }
        public DateTime? Approved { get; set; }
        public Employee Approver { get; set; }
        public int? ApproverId { get; set; }
        [NotMapped]
        public ICollection<Comment> ApproverComments { get; set; }
        public DateTime? Performed { get; set; }
        public Employee Performer { get; set; }
        public int? PerformerId { get; set; }
        [NotMapped]
        public ICollection<Comment> PerformerComments { get; set; }
        public Status Status { get; set; }
        public int StatusId { get; set; }
        [NotMapped]
        public ICollection<Employee> Performers { get; set; }
        [NotMapped]
        public ICollection<Employee> Testers { get; set; }

        public static void PrepareTicketsToView(List<Ticket> tickets)
        {
            foreach (var ticket in tickets)
            {
                PrepareTicketToView(ticket);
            }
        }

        public static void PrepareTicketToView(Ticket ticket)
        {
            using (var ctx = new SolutionsContext())
            {
                ticket.Author = Employee.GetEmployee(ticket.AuthorId);
                ticket.Tester = Employee.GetEmployee(ticket.TesterId);
                ticket.Approver = Employee.GetEmployee(ticket.ApproverId);
                ticket.Performer = Employee.GetEmployee(ticket.PerformerId);
                ticket.Operation = Operation.ById(ticket.OperationId);
                ticket.Priority = Priority.ById(ticket.PriorityId);
                ticket.Resource = Resource.ById(ticket.ResourceId);
                ticket.Status = Status.ById(ticket.StatusId);
                ticket.ApproverComments = Comment.ByType(ticket.Id, (int)Enums.CommentTypes.Approve);
                ticket.PerformerComments = Comment.ByType(ticket.Id, (int)Enums.CommentTypes.Perform);

                ticket.Performers = Employee.EmployeesByResourceId((int)Enums.Roles.PERFORMER, ticket.ResourceId);
                ticket.Testers = Employee.ByRoleRepo((int)Enums.Roles.TESTER);
            }
        }

        public static Ticket ById(int ticketId)
        {
            using (var ctx = new SolutionsContext())
            {
                return ctx.Tickets.First(x => x.Id == ticketId);
            }
        }

        public static List<Ticket> ByStatusRepo(int statusId)
        {
            using (var ctx = new SolutionsContext())
            {
                var tickets = ctx.Tickets
                    .Where(x => x.StatusId == statusId)
                    .OrderByDescending(x => x.Created)
                    .ToList();

                Ticket.PrepareTicketsToView(tickets);
                return tickets;
            }
        }

        public static List<Ticket> ClosedRepo(Employee employee, int roleId)
        {
            var from = DateTime.Now.AddMonths(-1);
            var closedStatuses = new List<int>()
            {
                (int)Enums.Statuses.DONE,
                (int)Enums.Statuses.DECLINED
            };

            using (var ctx = new SolutionsContext())
            {
                var tickets = new List<Ticket>();

                if (employee.HasRole((int)Enums.Roles.ADMINISTRATOR))
                {
                    tickets = ctx.Tickets
                        .Where(x => x.Created >= from &&
                        closedStatuses.Contains(x.StatusId))
                        .OrderByDescending(x => x.Created)
                        .AsNoTracking()
                        .ToList();
                }
                else
                {
                    if (roleId == (int)Enums.Roles.AUTHOR)
                    {
                        ctx.Tickets
                            .Where(x => x.AuthorId == employee.Id &&
                            x.Created >= from &&
                            closedStatuses.Contains(x.StatusId))
                            .OrderByDescending(x => x.Created)
                            .AsNoTracking()
                            .ToList();
                    }
                    else
                    {
                        var employeeResources = employee.Permissions
                            .Where(x => x.RoleId == roleId)
                            .Select(p => p.ResourceId)
                            .ToList();

                        tickets = ctx.Tickets
                            .Where(x => employeeResources.Contains(x.ResourceId) &&
                                   x.Created >= from &&
                                   closedStatuses.Contains(x.StatusId))
                            .AsNoTracking()
                            .ToList();
                    }
                }

                Ticket.PrepareTicketsToView(tickets);
                return tickets;
            }
        }

        public static List<Ticket> ActivePriorityRepo(int priorityId)
        {
            using (var ctx = new SolutionsContext())
            {
                var tickets = ctx.Tickets
                    .Where(x => x.PriorityId == priorityId &&
                    x.StatusId != (int)Enums.Statuses.DONE &&
                    x.StatusId != (int)Enums.Statuses.DECLINED &&
                    x.StatusId != (int)Enums.Statuses.DELETED)
                    .OrderByDescending(x => x.Created)
                    .ToList();

                Ticket.PrepareTicketsToView(tickets);
                return tickets;
            }
        }

        public static List<Ticket> ActiveRepo()
        {
            using (var ctx = new SolutionsContext())
            {
                var tickets = ctx.Tickets
                    .Where(x => x.StatusId != (int)Enums.Statuses.DONE &&
                    x.StatusId != (int)Enums.Statuses.DECLINED &&
                    x.StatusId != (int)Enums.Statuses.DELETED)
                    .OrderByDescending(x => x.Created)
                    .AsNoTracking()
                    .ToList();

                Ticket.PrepareTicketsToView(tickets);
                return tickets;
            }
        }

        public static List<Ticket> AuthorRepo(List<int> statusIds, Employee employee)
        {
            List<Ticket> tickets;

            using (var ctx = new SolutionsContext())
            {
                if (employee.HasRole((int)Enums.Roles.ADMINISTRATOR))
                {
                    tickets = ctx.Tickets
                        .Where(x => statusIds.Contains(x.StatusId))
                        .OrderByDescending(x => x.Created)
                        .ToList();
                }
                else
                {
                    tickets = ctx.Tickets
                        .Where(x => x.AuthorId == employee.Id &&
                        statusIds.Contains(x.StatusId))
                        .OrderByDescending(x => x.Created)
                        .ToList();
                }
            }

            Ticket.PrepareTicketsToView(tickets);
            return tickets;
        }

        public static List<Ticket> ByEmployeeRepo(int statusId, int roleId, Employee employee)
        {
            List<Ticket> tickets;
            if (employee.HasRole((int)Enums.Roles.ADMINISTRATOR))
                tickets = Ticket.ByStatusRepo(statusId);
            else
            {
                var resources = employee.Permissions
                    .Where(x => x.RoleId == roleId)
                    .Select(p => p.ResourceId)
                    .ToList();

                using (var ctx = new SolutionsContext())
                {
                    tickets = ctx.Tickets
                        .Where(x => x.StatusId == statusId && resources.Contains(x.ResourceId))
                        .OrderByDescending(x => x.Created)
                        .ToList();
                }
            }

            Ticket.PrepareTicketsToView(tickets);
            return tickets;
        }

        public static void Assign(int ticketId, int employeeId, string comment)
        {
            using (var ctx = new SolutionsContext())
            {
                var ticket = ctx.Tickets.First(x => x.Id == ticketId);
                ticket.PerformerId = employeeId;
                ticket.StatusId = (int)Enums.Statuses.ON_PERFORM;

                ctx.SaveChanges();

                Mail.Send(ticket, comment);
            }
        }

        public static void Approve(int ticketId, int approverId, string comment)
        {
            using (var ctx = new SolutionsContext())
            {
                var ticket = ctx.Tickets.First(x => x.Id == ticketId);
                ticket.Approved = DateTime.Now;
                ticket.ApproverId = approverId;

                if (ticket.PriorityId == (int)Enums.Priorities.C)
                    ticket.StatusId = (int)Enums.Statuses.HOLD;
                else
                    ticket.StatusId = (int)Enums.Statuses.ON_PERFORM;

                ctx.SaveChanges();

                Mail.Send(ticket, comment);
            }
        }

        public static void Reject(int ticketId, int employeeId, string comment)
        {
            using (var ctx = new SolutionsContext())
            {
                var ticket = ctx.Tickets.First(x => x.Id == ticketId);
                ticket.Approved = DateTime.Now;
                ticket.ApproverId = employeeId;
                ticket.StatusId = (int)Enums.Statuses.NOT_APPROVED;

                ctx.SaveChanges();

                Mail.Send(ticket, comment);
            }
        }

        public static void Reapprove(Ticket ticketData, string comment)
        {
            using (var ctx = new SolutionsContext())
            {
                var ticket = ctx.Tickets.First(x => x.Id == ticketData.Id);
                ticket.TechnicalTask = ticketData.TechnicalTask;
                ticket.Desc = ticketData.Desc;
                ticket.FullDesc = ticketData.FullDesc;
                ticket.Instructions = ticketData.Instructions;
                ticket.StatusId = (int)Enums.Statuses.ON_APPROVE;

                ctx.SaveChanges();

                Mail.Send(ticket, comment);
            }
        }

        public static void Close(int ticketId)
        {
            using (var ctx = new SolutionsContext())
            {
                var ticket = ctx.Tickets.First(x => x.Id == ticketId);
                ticket.StatusId = (int)Enums.Statuses.CLOSED;

                ctx.SaveChanges();

                Mail.Send(ticket);
            }
        }

        public static void Perform(int ticketId, int employeeId, string comment)
        {
            using (var ctx = new SolutionsContext())
            {
                var ticket = ctx.Tickets.First(x => x.Id == ticketId);
                ticket.Performed = DateTime.Now;
                ticket.StatusId = (int)Enums.Statuses.DONE;

                ctx.SaveChanges();

                Mail.Send(ticket, comment);
            }
        }

        public static void Hold(int ticketId, int employeeId, string comment)
        {
            using (var ctx = new SolutionsContext())
            {
                var ticket = ctx.Tickets.First(x => x.Id == ticketId);
                ticket.StatusId = (int)Enums.Statuses.HOLD;

                ctx.SaveChanges();

                Mail.Send(ticket, comment);
            }
        }

        public static void Decline(int ticketId, int employeeId, string comment)
        {
            using (var ctx = new SolutionsContext())
            {
                var ticket = ctx.Tickets.First(x => x.Id == ticketId);
                ticket.Performed = DateTime.Now;
                ticket.StatusId = (int)Enums.Statuses.DECLINED;

                ctx.SaveChanges();

                Mail.Send(ticket, comment);
            }
        }

        public static List<Ticket> History(string solution, int currentTicketId)
        {
            var historyKeywords = HistoryKeywords(solution);
            var historyTickets = new List<Ticket>();

            if (historyKeywords != null)
            {
                using (var ctx = new SolutionsContext())
                {
                    foreach (var keyword in historyKeywords)
                    {
                        var keywordHistory = ctx.Tickets
                            .Where(x => x.Solutions.Contains(keyword) && x.Id != currentTicketId)
                            .AsNoTracking()
                            .ToList();

                        historyTickets.AddRange(keywordHistory);
                    }
                }
            }

            if (historyTickets.Count == 0)
                return null;

            foreach (var ticket in historyTickets)
            {
                ticket.Author = Employee.GetEmployee(ticket.AuthorId);
                ticket.Tester = Employee.GetEmployee(ticket.TesterId);
                ticket.Approver = Employee.GetEmployee(ticket.ApproverId);
                ticket.Performer = Employee.GetEmployee(ticket.PerformerId);
                ticket.Operation = Operation.ById(ticket.OperationId);
                ticket.Priority = Priority.ById(ticket.PriorityId);
                ticket.Resource = Resource.ById(ticket.ResourceId);
                ticket.Status = Status.ById(ticket.StatusId);
            }

            historyTickets = historyTickets
                .OrderByDescending(x => x.Created)
                .ToList();

            return historyTickets;
        }

        private static List<string> HistoryKeywords(string solution)
        {
            var keywords = solution.Split('-', '.', ',', '(', ')', ' ');
            keywords = keywords.Where(x => !string.IsNullOrEmpty(x) && x != "KB").ToArray();
            keywords = keywords.Distinct().ToArray();

            var sb = new StringBuilder();
            foreach (var word in keywords)
            {
                var trimmed = word.Trim();
                if (trimmed != string.Empty)
                {
                    var firstChar = trimmed.First().ToString();
                    if (Regex.IsMatch(firstChar, "[a-zA-Z]") || Regex.IsMatch(firstChar, "\\d"))
                        sb.Append(word == keywords.Last() ? trimmed : trimmed + ',');
                }
            }

            var keywordString = sb.ToString();
            if (keywordString != string.Empty)
                return keywordString.Split(',').ToList();

            return null;
        }
    }
}