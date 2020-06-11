using System.Collections.Generic;
using ERPSolutions_EF.Models;

namespace ERPSolutions_EF.ViewModels
{
    public class PermissionsViewModel
    {
        public int Id { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        public bool Author { get; set; }
        public bool SendMail { get; set; }
        public bool Tester { get; set; }
        public bool Approver { get; set; }
        public bool Performer { get; set; }
        public bool Administrator { get; set; }

        public List<Resource> ApproveResources { get; set; }
        public List<Resource> PerformResources { get; set; }

        public List<int> SelectedApproveResources { get; set; }
        public List<int> SelectedPerformResources { get; set; }
        public List<Resource> ResourceRepo { get; set; }
    }
}