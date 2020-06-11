using System.Linq;
using ERPSolutions_EF.DAL;

namespace ERPSolutions_EF.Models
{
    public class Priority
    {
        public int Id { get; set; }
        public string Literal { get; set; }
        public string Title { get; set; }

        public static Priority ById(int id)
        {
            using (var ctx = new SolutionsContext())
            {
                return ctx.Priorities.First(x => x.Id == id);
            }
        }
    }
}