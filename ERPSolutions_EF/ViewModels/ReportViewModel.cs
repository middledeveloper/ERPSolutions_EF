using System;
using System.Collections.Generic;
using ERPSolutions_EF.Models;

namespace ERPSolutions_EF.ViewModels
{
    public class ReportViewModel
    {
        public int Id { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public string SolutionNumber { get; set; }
        public List<Ticket> Tickets { get; set; }
    }
}