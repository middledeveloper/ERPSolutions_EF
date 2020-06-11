using System.Linq;
using ERPSolutions_EF.DAL;

namespace ERPSolutions_EF.Models
{
    public class Operation
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public static Operation ById(int id)
        {
            using (var ctx = new SolutionsContext())
            {
                return ctx.Operations.First(x => x.Id == id);
            }
        }
    }
}