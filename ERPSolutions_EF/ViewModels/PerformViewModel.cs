using System.Collections.Generic;
using ERPSolutions_EF.Models;

namespace ERPSolutions_EF.ViewModels
{
    public class PerformViewModel
    {
        public int Id { get; set; }
        public Employee Performer { get; set; }
        public ICollection<Ticket> HoldTickets { get; set; }
        public ICollection<Ticket> PerformTickets { get; set; }
        public ICollection<Ticket> ClosedTickets { get; set; }
    }
}