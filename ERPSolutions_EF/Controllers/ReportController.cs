using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ClosedXML.Excel;
using ERPSolutions_EF.DAL;
using ERPSolutions_EF.Models;
using ERPSolutions_EF.ViewModels;

namespace ERPSolutions_EF.Controllers
{
    public class ReportController : Controller
    {
        public ActionResult Index()
        {
            var employee = Employee.GetEmployee(User.Identity.Name);
            if (employee == null)
                return View("Block");

            RemoveOldFiles();

            var model = new ReportViewModel()
            {
                PeriodStart = DateTime.Now.Date,
                PeriodEnd = DateTime.Now.Date.AddDays(1)
            };

            return View(model);
        }

        public ActionResult GeneratePeriodReport(ReportViewModel rvm, string excel)
        {
            if (rvm.PeriodStart == new DateTime())
                rvm.PeriodStart = DateTime.Now.AddMonths(-1);
            if (rvm.PeriodEnd == new DateTime())
                rvm.PeriodEnd = DateTime.Now;

            if (rvm.PeriodStart < rvm.PeriodEnd)
            {
                using (var ctx = new SolutionsContext())
                {
                    rvm.Tickets = ctx.Tickets.Where(x =>
                        x.Created >= rvm.PeriodStart &&
                        x.Created <= rvm.PeriodEnd).ToList();

                    PrepareTicketList(rvm.Tickets);

                    if (rvm.Tickets.Count > 0)
                    {
                        if (excel == null)
                        {
                            return View("Index", rvm);
                        }

                        var filePath = GenerateXLSX(rvm.Tickets);
                        return RedirectToAction("Download", new { path = filePath });
                    }
                    else
                    {
                        ModelState.AddModelError("PeriodStart", "В указанный период заявок создано не было");
                        return View("Index", rvm);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("PeriodStart", "Дата начала периода больше, чем дата окончания (либо даты равны)");
                return View("Index", rvm);
            }
        }

        public ActionResult GenerateSolutionReport(ReportViewModel rvm, string excel)
        {
            if (rvm.PeriodStart == new DateTime())
                rvm.PeriodStart = DateTime.Now.AddMonths(-1);
            if (rvm.PeriodEnd == new DateTime())
                rvm.PeriodEnd = DateTime.Now;

            if (!string.IsNullOrEmpty(rvm.SolutionNumber))
            {
                using (var ctx = new SolutionsContext())
                {
                    rvm.Tickets = ctx.Tickets.Where(x => x.Solutions.Contains(rvm.SolutionNumber)).ToList();
                    if (rvm.Tickets.Count > 0)
                    {
                        PrepareTicketList(rvm.Tickets);

                        if (excel == null)
                        {
                            return View("Index", rvm);
                        }

                        var filePath = GenerateXLSX(rvm.Tickets);
                        return RedirectToAction("Download", new { path = filePath });
                    }
                    else
                    {
                        ModelState.AddModelError("SolutionNumber", "По заданному номеру результатов не обнаружено");
                        return View("Index", rvm);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("SolutionNumber", "Номер решения не указан");
                return View("Index", rvm);
            }
        }

        public ActionResult ReportTicketList(List<Ticket> tickets)
        {
            return PartialView("_DefaultTicketList", tickets);
        }

        private static void PrepareTicketList(List<Ticket> tickets)
        {
            using (var ctx = new SolutionsContext())
            {
                var resources = ctx.Resources.ToList();
                var statuses = ctx.Statuses.ToList();
                var operations = ctx.Operations.ToList();
            }

            foreach (var ticket in tickets)
            {
                ticket.Author = Employee.GetEmployee(ticket.AuthorId);
                ticket.Priority = Priority.ById(ticket.PriorityId);
                ticket.Operation = Operation.ById(ticket.OperationId);
                ticket.Resource = Resource.ById(ticket.OperationId);
                ticket.Status = Status.ById(ticket.StatusId);

                if (ticket.TesterId != null)
                    ticket.Tester = Employee.GetEmployee(ticket.TesterId);
                if (ticket.ApproverId != null)
                    ticket.Approver = Employee.GetEmployee(ticket.ApproverId);
                if (ticket.PerformerId != null)
                    ticket.Performer = Employee.GetEmployee(ticket.PerformerId);

                ticket.ApproverComments = Comment.ByType(ticket.Id, (int)Enums.CommentTypes.Approve);
                ticket.PerformerComments = Comment.ByType(ticket.Id, (int)Enums.CommentTypes.Perform);
            }
        }

        private string GenerateXLSX(List<Ticket> tickets)
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Report tab");
            ws.Column(2).Width = 15;
            ws.Column(3).Width = 20;
            ws.Column(4).Width = 25;
            ws.Column(5).Width = 20;
            ws.Column(6).Width = 15;
            ws.Column(7).Width = 30;
            ws.Column(8).Width = 30;
            ws.Column(9).Width = 80;
            ws.Column(10).Width = 50;
            ws.Column(11).Width = 30;
            ws.Column(12).Width = 25;
            ws.Column(13).Width = 15;
            ws.Column(14).Width = 15;
            ws.Column(15).Width = 15;
            ws.Column(16).Width = 25;
            ws.Column(17).Width = 25;
            ws.Column(18).Width = 25;
            ws.Column(19).Width = 40;
            ws.Column(20).Width = 15;

            ws.Cell("A1").Value = "ИД";
            ws.Cell("B1").Value = "Действие";
            ws.Cell("C1").Value = "Класс срочности";
            ws.Cell("D1").Value = "Обоснование срочности";
            ws.Cell("E1").Value = "Информационная система";
            ws.Cell("F1").Value = "Статус";
            ws.Cell("G1").Value = "Номера решений";
            ws.Cell("H1").Value = "Краткая информация";
            ws.Cell("I1").Value = "Подробная информация";
            ws.Cell("J1").Value = "Инструкции";
            ws.Cell("K1").Value = "Ссылка на ТЗ";
            ws.Cell("L1").Value = "Кем протестировано";
            ws.Cell("M1").Value = "Создано";
            ws.Cell("N1").Value = "Согласовано";
            ws.Cell("O1").Value = "Выполнено";
            ws.Cell("P1").Value = "Автор";
            ws.Cell("Q1").Value = "Согласовант";
            ws.Cell("R1").Value = "Исполнитель";

            var activeRow = 2;

            for (var i = 0; i < tickets.Count; i++)
            {
                ws.Cell("A" + activeRow).Value = tickets[i].Id;
                ws.Cell("B" + activeRow).Value = tickets[i].Operation.Title;
                ws.Cell("C" + activeRow).Value = tickets[i].Priority.Literal;
                ws.Cell("D" + activeRow).Value = tickets[i].PriorityDesc ?? "-";
                ws.Cell("E" + activeRow).Value = tickets[i].Resource.Title;
                ws.Cell("F" + activeRow).Value = tickets[i].Status.Title;
                ws.Cell("G" + activeRow).Value = tickets[i].Solutions;
                ws.Cell("H" + activeRow).Value = tickets[i].Desc;
                ws.Cell("I" + activeRow).Value = tickets[i].FullDesc ?? "-";
                ws.Cell("J" + activeRow).Value = tickets[i].Instructions ?? "-";
                ws.Cell("K" + activeRow).Value = tickets[i].TechnicalTask ?? "-";
                ws.Cell("L" + activeRow).Value = tickets[i].Tester == null ? "-" : tickets[i].Tester.Name;
                ws.Cell("M" + activeRow).Value = tickets[i].Created;
                ws.Cell("N" + activeRow).Value = tickets[i].Approved == null ? "-" : tickets[i].Approved.ToString();
                ws.Cell("O" + activeRow).Value = tickets[i].Performed == null ? "-" : tickets[i].Performed.ToString();
                ws.Cell("P" + activeRow).Value = tickets[i].Author == null ? "-" : tickets[i].Author.Name;
                ws.Cell("Q" + activeRow).Value = tickets[i].Approver == null ? "-" : tickets[i].Approver.Name;
                ws.Cell("R" + activeRow).Value = tickets[i].Performer == null ? "-" : tickets[i].Performer.Name;
                activeRow++;

                if (tickets[i].ApproverComments.Count > 0)
                {
                    foreach (var comment in tickets[i].ApproverComments)
                    {
                        MergeCells(ws, activeRow);
                        ws.Cell("A" + activeRow).Value = string.Format("{0} ({1}, {2})",
                            comment.Text, comment.Author.Name, comment.Date);

                        activeRow++;

                        if (comment.Answer != null)
                        {
                            MergeCells(ws, activeRow);
                            ws.Cell("A" + activeRow).Value = string.Format("{0} ({1})",
                                comment.Answer, tickets[i].Author.Name);

                            activeRow++;
                        }
                    }
                }

                if (tickets[i].PerformerComments.Count > 0)
                {
                    foreach (var comment in tickets[i].PerformerComments)
                    {
                        MergeCells(ws, activeRow);
                        ws.Cell("A" + activeRow).Value = string.Format("{0} ({1}, {2})",
                            comment.Text, comment.Author.Name, comment.Date);

                        activeRow++;

                        if (comment.Answer != null)
                        {
                            MergeCells(ws, activeRow);
                            ws.Cell("A" + activeRow).Value = string.Format("{0} ({1})",
                                comment.Answer, tickets[i].Author.Name);

                            activeRow++;
                        }
                    }
                }
            }

            ws.RangeUsed().Style.Font.FontSize = 10;
            ws.CellsUsed().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.CellsUsed().Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws.CellsUsed().Style.Alignment.WrapText = true;
            ws.CellsUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            ws.Range("A1:R1").Style.Fill.BackgroundColor = XLColor.Gray;
            ws.Range("A1:R1").Style.Font.FontColor = XLColor.White;
            ws.Range("A1:R1").Style.Font.Bold = true;

            ws.SheetView.FreezeRows(1);

            var path = "~/Reports/" + DateTime.Now.ToString("dd.MM.yyyy_HH.mm.ss") + "_.xlsx";
            wb.SaveAs(Server.MapPath(path));

            return path;
        }

        private static void MergeCells(IXLWorksheet ws, int activeRow)
        {
            ws.Cell("A" + activeRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Range("A" + activeRow + ":R" + activeRow).Merge();
        }

        public FileResult Download(string path)
        {
            var name = Path.GetFileName(Server.MapPath(path));
            var doc = System.IO.File.ReadAllBytes(Server.MapPath(path));

            return File(doc, "application/vnd.ms-excel", name);
        }

        private void RemoveOldFiles()
        {
            var dir = new DirectoryInfo(Server.MapPath("~/Reports"));
            var files = dir.GetFiles().Where(x => x.Name != "readme.txt").ToArray();
            if (files.Count() > 0)
            {
                foreach (var f in files)
                {
                    if (f.CreationTime < DateTime.Now.AddHours(-1))
                    {
                        try { f.Delete(); }
                        catch (IOException)
                        {
                            // do nothing
                        }
                    }
                }
            }
        }
    }
}