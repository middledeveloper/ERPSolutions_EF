using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using ERPSolutions_EF.Models;

namespace ERPSolutions_EF.DAL
{
    public class Mail
    {
        public static void Send(Ticket ticket, string comment = null)
        {
            Ticket.PrepareTicketToView(ticket);

            var msg = new MailMessage()
            {
                From = new MailAddress("ERPSolutions@domain.ru"),
                Subject = ticket.Status.Title + " | " + ticket.Priority.Literal + " |",
                Body = Message(ticket, comment),
                IsBodyHtml = true
            };

            var ticketEmails = TicketEmails(ticket);
            ticketEmails.ForEach(x => msg.To.Add(new MailAddress(x)));

            if (ticket.PriorityId == (int)Enums.Priorities.A)
                msg.Priority = MailPriority.High;

            var client = new SmtpClient("owa.domain.ru", 25)
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true
            };

            client.Send(msg);
        }

        private static string Message(Ticket ticket, string comment)
        {
            var body = new StringBuilder();
            body.Append("<div style='font-family:Calibri, Arial, Tahoma; font-size:11pt;'>");
            body.Append("Добрый день!<br /><br />");
            body.Append(string.Format("Заявка класса {0} от {1} ({2} {3}) ",
                ticket.Priority.Literal, ticket.Created, ticket.Operation.Title.ToLower(), ticket.Solutions));

            var appUrl = "http://host-name:port/";

            switch (ticket.StatusId)
            {
                case (int)Enums.Statuses.ON_APPROVE:
                    appUrl = appUrl + "approve";
                    body.Append("находится на согласовании.<br />");
                    break;
                case (int)Enums.Statuses.NOT_APPROVED:
                    body.Append("находится на доработке.<br />");
                    if (comment != string.Empty && comment != null)
                        body.Append(string.Format("Комментарий согласующего:<br />{0}<br /><br />", comment));
                    break;
                case (int)Enums.Statuses.ON_PERFORM:
                    appUrl = appUrl + "perform";
                    body.Append("находится на исполнении.<br />");
                    if (comment != string.Empty && comment != null)
                        body.Append(string.Format("Комментарий согласующего:<br />{0}<br /><br />", comment));
                    break;
                case (int)Enums.Statuses.HOLD:
                    appUrl = appUrl + "perform";
                    body.Append("в режиме паузы.<br />");
                    if (comment != string.Empty && comment != null)
                        body.Append(string.Format("Комментарий исполнителя:<br />{0}<br /><br />", comment));
                    break;
                case (int)Enums.Statuses.DONE:
                    body.Append("исполнена.<br />");
                    if (comment != string.Empty && comment != null)
                        body.Append(string.Format("Комментарий исполнителя:<br />{0}<br /><br />", comment));
                    break;
                case (int)Enums.Statuses.DECLINED:
                    body.Append("отклонена.<br />");
                    if (comment != string.Empty && comment != null)
                        body.Append(string.Format("Комментарий исполнителя:<br />{0}<br /><br />", comment));
                    break;
                case (int)Enums.Statuses.CLOSED:
                    body.Append("закрыта инициатором.<br />");
                    break;
            }

            if (ticket.StatusId != (int)Enums.Statuses.CLOSED &&
                    ticket.StatusId != (int)Enums.Statuses.DECLINED &&
                        ticket.StatusId != (int)Enums.Statuses.DONE)
                body.Append("Это сообщение является напоминанием о необходимости действия по заявке.<br /><br />");

            body.Append(string.Format("<a href='{0}'>Перейти к приложению</a><br /><br />", appUrl));
            body.Append("<br /><br />С уважением,<br />Подразделение</div>");
            return body.ToString();
        }

        private static List<string> TicketEmails(Ticket ticket)
        {
            var emails = new List<string>
            {
                ActiveDirectory.GetEmail(Employee.GetEmployee(ticket.AuthorId).Account)
            };

            switch (ticket.StatusId)
            {
                case (int)Enums.Statuses.ON_APPROVE:
                case (int)Enums.Statuses.NOT_APPROVED:
                    ApproverEmails(ticket, emails);
                    break;
                case (int)Enums.Statuses.ON_PERFORM:
                case (int)Enums.Statuses.HOLD:
                case (int)Enums.Statuses.DONE:
                case (int)Enums.Statuses.DECLINED:
                case (int)Enums.Statuses.CLOSED:
                    ApproverEmails(ticket, emails);
                    PerformerEmails(ticket, emails);
                    break;
            }

            return emails.Where(x => x != null).Distinct().ToList();
        }

        private static void ApproverEmails(Ticket ticket, List<string> emails)
        {
            if (ticket.ApproverId != null)
                emails.Add(ActiveDirectory.GetEmail(Employee.GetEmployee(ticket.ApproverId).Account));
            else
            {
                var employees = Employee.EmployeesByResourceId((int)Enums.Roles.APPROVER, ticket.ResourceId);
                employees
                    .Where(x => x.SendMail)
                    .ToList()
                    .ForEach(x => emails.Add(ActiveDirectory.GetEmail(x.Account)));
            }
        }

        private static void PerformerEmails(Ticket ticket, List<string> emails)
        {
            if (ticket.PerformerId != null)
                emails.Add(ActiveDirectory.GetEmail(Employee.GetEmployee(ticket.PerformerId).Account));
            else
            {
                var employees = Employee.EmployeesByResourceId((int)Enums.Roles.PERFORMER, ticket.ResourceId);
                employees
                    .Where(x => x.SendMail)
                    .ToList()
                    .ForEach(x => emails.Add(ActiveDirectory.GetEmail(x.Account)));
            }
        }
    }
}