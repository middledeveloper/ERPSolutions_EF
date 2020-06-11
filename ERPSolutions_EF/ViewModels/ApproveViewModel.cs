using System.Collections.Generic;
using ERPSolutions_EF.Models;

namespace ERPSolutions_EF.ViewModels
{
    public class ApproveViewModel
    {
        public int Id { get; set; }
        public Employee Approver { get; set; }
        public ICollection<Ticket> ApprovalTickets { get; set; }
        public ICollection<Ticket> PerformingTickets { get; set; }
        public ICollection<Ticket> ClosedTickets { get; set; }
    }
}