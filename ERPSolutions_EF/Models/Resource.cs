using System.Linq;
using ERPSolutions_EF.DAL;

namespace ERPSolutions_EF.Models
{
    public class Resource
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public static Resource ById(int id)
        {
            using (var ctx = new SolutionsContext())
            {
                return ctx.Resources.First(x => x.Id == id);
            }
        }
    }
}