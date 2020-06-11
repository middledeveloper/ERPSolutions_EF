using System.Linq;
using ERPSolutions_EF.DAL;

namespace ERPSolutions_EF.Models
{
    public class Status
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public static Status ById(int id)
        {
            using (var ctx = new SolutionsContext())
            {
                return ctx.Statuses.First(x => x.Id == id);
            }
        }
    }
}