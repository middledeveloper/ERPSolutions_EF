using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ERPSolutions_EF.DAL;

namespace ERPSolutions_EF.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Account { get; set; }
        public bool Active { get; set; }
        public bool SendMail { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public ICollection<Permission> Permissions { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string Name { get; set; }

        public bool HasRole(int roleId)
        {
            foreach (var permission in this.Permissions)
            {
                if (permission.RoleId == roleId)
                    return true;
            }

            return false;
        }

        public bool HasRoleResource(int roleId, int resourceId)
        {
            foreach (var permission in this.Permissions)
            {
                if (permission.RoleId == roleId && permission.ResourceId == resourceId)
                    return true;
            }

            return false;
        }

        public static List<Employee> ByRoleRepo(int roleId)
        {
            using (var ctx = new SolutionsContext())
            {
                var employeeIds = ctx.Permissions
                .Where(x => x.RoleId == roleId)
                .Select(x => x.EmployeeId)
                .ToList();

                var employees = ctx.Employees
                    .Where(x => x.Active && employeeIds.Contains(x.Id))
                    .ToList();

                employees.ForEach(x => x.GetData());

                return employees.OrderBy(x => x.Name).ToList();
            }
        }

        public static Employee GetEmployee(int? id)
        {
            if (id == null)
                return null;

            using (var ctx = new SolutionsContext())
            {
                var user = ctx.Employees.FirstOrDefault(x => x.Id == id);
                if (user == null)
                    return null;

                user.GetData();
                return user;
            }

        }

        public static Employee GetEmployee(string account)
        {
            using (var ctx = new SolutionsContext())
            {
                var user = ctx.Employees.FirstOrDefault(x => x.Account == account);
                if (user == null)
                    return null;

                user.GetData();
                return user;
            }
        }

        public static List<Employee> EmployeesByResourceId(int roleId, int resourceId)
        {
            using (var ctx = new SolutionsContext())
            {
                var employeeRepo = ctx.Employees.Where(x => x.Active).ToList();
                employeeRepo.ForEach(x => x.GetData());

                var employeesByResource = new List<Employee>();
                foreach (var employee in employeeRepo)
                {
                    if (employee.Permissions.FirstOrDefault(x =>
                        x.RoleId == roleId &&
                        x.ResourceId == resourceId) != null)
                    {
                        employeesByResource.Add(employee);
                    }
                }

                return employeesByResource;
            }
        }

        public static void EmployeeActivity(int employeeId, bool isActive)
        {
            var employee = Employee.GetEmployee(employeeId);
            if (employee != null)
            {
                using (var ctx = new SolutionsContext())
                {
                    if (!isActive)
                    {
                        if (employee.Permissions != null)
                        {
                            var permissions = ctx.Permissions
                                .Where(x => x.EmployeeId == employeeId)
                                .ToList();

                            ctx.Permissions.RemoveRange(permissions);
                            ctx.SaveChanges();
                        }
                    }

                    ctx.Employees.First(x => x.Id == employeeId).Active = isActive;
                    ctx.SaveChanges();
                }
            }
        }

        public void GetData()
        {
            using (var ctx = new SolutionsContext())
            {
                this.Name = ActiveDirectory.GetName(this.Account);
                this.Permissions = ctx.Permissions
                    .Where(x => x.EmployeeId == this.Id)
                    .AsNoTracking()
                    .ToList();
            }
        }
    }
}