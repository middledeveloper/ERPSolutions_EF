namespace ERPSolutions_EF.Models
{
    public class Permission
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int RoleId { get; set; }
        public int ResourceId { get; set; }
    }
}