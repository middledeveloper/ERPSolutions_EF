using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERPSolutions_EF.Models;

namespace ERPSolutions_EF.ViewModels
{
    public class InitiateViewModel
    {
        public int Id { get; set; }
        public int ResourceId { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public int OperationId { get; set; }
        [Required(ErrorMessage = "Укажите номер")]
        public string Solutions { get; set; }
        [AllowHtml]
        public string TechnicalTask { get; set; }
        [AllowHtml]
        [Required(ErrorMessage = "Укажите краткое описание")]
        public string Desc { get; set; }
        [AllowHtml]
        public string FullDesc { get; set; }
        [AllowHtml]
        public string Instructions { get; set; }
        public int PriorityId { get; set; }
        public string PriorityDesc { get; set; }
        public int TesterId { get; set; }

        public List<Resource> Resources { get; set; }
        public List<Operation> Operations { get; set; }
        public List<Priority> Priorities { get; set; }
        public List<Employee> Testers { get; set; }

        public List<Ticket> ActiveTickets { get; set; }
        public List<Ticket> TroubleTickets { get; set; }
        public List<Ticket> ClosedTickets { get; set; }
    }
}