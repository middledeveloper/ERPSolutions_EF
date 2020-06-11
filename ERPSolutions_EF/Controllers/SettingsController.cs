using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ERPSolutions_EF.DAL;
using ERPSolutions_EF.Models;
using ERPSolutions_EF.ViewModels;

namespace ERPSolutions_EF.Controllers
{
    public class SettingsController : Controller
    {
        public ActionResult Index()
        {
            using (var ctx = new SolutionsContext())
            {
                var permissionRepo = new List<PermissionsViewModel>();

                var employees = ctx.Employees.Where(x => x.Active).ToList();
                employees.ForEach(x => x.GetData());
                employees = employees.OrderBy(x => x.Name).ToList();

                foreach (var employee in employees)
                {
                    var model = new PermissionsViewModel()
                    {
                        Id = employee.Id,
                        Account = employee.Account,
                        Name = employee.Name,
                        SendMail = employee.SendMail,
                        Author = employee.HasRole((int)Enums.Roles.AUTHOR),
                        Tester = employee.HasRole((int)Enums.Roles.TESTER),
                        Approver = employee.HasRole((int)Enums.Roles.APPROVER),
                        Performer = employee.HasRole((int)Enums.Roles.PERFORMER),
                        Administrator = employee.HasRole((int)Enums.Roles.ADMINISTRATOR)
                    };

                    if (model.Approver)
                    {
                        model.ApproveResources =
                            BindPermissions(employee.Permissions.ToList(), (int)Enums.Roles.APPROVER);
                    }

                    if (model.Performer)
                    {
                        model.PerformResources =
                            BindPermissions(employee.Permissions.ToList(), (int)Enums.Roles.PERFORMER);
                    }

                    permissionRepo.Add(model);
                }

                return View(permissionRepo);
            }
        }

        public ActionResult AddEmployee()
        {
            using (var ctx = new SolutionsContext())
            {
                var model = new PermissionsViewModel()
                {
                    Author = true,
                    Tester = true,
                    Approver = false,
                    Performer = false,
                    Administrator = false,
                    ResourceRepo = ctx.Resources.ToList(),
                    SelectedApproveResources = new List<int>(),
                    SelectedPerformResources = new List<int>()
                };

                return View("EmployeeForm", model);
            }
        }

        public ActionResult SaveEmployeeForm(PermissionsViewModel model)
        {
            if (model.Id == 0)
            {
                if (model.Account == null)
                {
                    using (var ctx = new SolutionsContext())
                        model.ResourceRepo = ctx.Resources.ToList();

                    model.SelectedApproveResources = new List<int>();
                    model.SelectedPerformResources = new List<int>();

                    ModelState.AddModelError("Account", "Не указана учетная запись");
                    return View("EmployeeForm", model);
                }

                var employee = Employee.GetEmployee(model.Account);
                if (employee == null)
                    InsertEmployee(model);
                else
                {
                    Employee.EmployeeActivity(employee.Id, true);
                    model.Id = employee.Id;
                    UpdateEmployee(model);
                }
            }
            else
                UpdateEmployee(model);

            return RedirectToAction("Index");
        }

        public ActionResult EditEmployee(int id)
        {
            using (var ctx = new SolutionsContext())
            {
                var employee = Employee.GetEmployee(id);
                var model = new PermissionsViewModel()
                {
                    Id = employee.Id,
                    Account = employee.Account,
                    SendMail = employee.SendMail,
                    ResourceRepo = ctx.Resources.ToList(),
                    SelectedApproveResources = new List<int>(),
                    SelectedPerformResources = new List<int>()
                };

                if (employee.HasRole((int)Enums.Roles.AUTHOR))
                    model.Author = true;

                if (employee.HasRole((int)Enums.Roles.TESTER))
                    model.Tester = true;

                if (employee.HasRole((int)Enums.Roles.APPROVER))
                    CollectResources(employee, (int)Enums.Roles.APPROVER, ref model);

                if (employee.HasRole((int)Enums.Roles.PERFORMER))
                    CollectResources(employee, (int)Enums.Roles.PERFORMER, ref model);

                if (employee.HasRole((int)Enums.Roles.ADMINISTRATOR))
                    model.Administrator = true;

                return View("EmployeeForm", model);
            }
        }

        public ActionResult RemoveEmployee(int employeeId)
        {
            Employee.EmployeeActivity(employeeId, false);

            return RedirectToAction("Index");
        }

        private void CollectResources(Employee employee, int roleId, ref PermissionsViewModel model)
        {
            var permissions = employee.Permissions.Where(x => x.RoleId == roleId).ToList();

            foreach (var permission in permissions)
                if (roleId == (int)Enums.Roles.APPROVER)
                    model.SelectedApproveResources.Add(permission.ResourceId);
                else if (roleId == (int)Enums.Roles.PERFORMER)
                    model.SelectedPerformResources.Add(permission.ResourceId);
        }

        private void InsertEmployee(PermissionsViewModel permissions)
        {
            using (var ctx = new SolutionsContext())
            {
                var employee = new Employee()
                {
                    Account = AccountFormat(permissions.Account),
                    Active = true,
                    SendMail = permissions.SendMail
                };

                ctx.Employees.Add(employee);
                ctx.SaveChanges();

                if (permissions.Author)
                    InsertPermission(ctx, employee.Id, (int)Enums.Roles.AUTHOR, 0);

                if (permissions.Tester)
                    InsertPermission(ctx, employee.Id, (int)Enums.Roles.TESTER, 0);

                if (permissions.SelectedApproveResources != null)
                {
                    InsertPermissions(
                        ctx,
                        employee.Id,
                        (int)Enums.Roles.APPROVER,
                        permissions.SelectedApproveResources);
                }

                if (permissions.SelectedPerformResources != null)
                    InsertPermissions(
                        ctx,
                        employee.Id,
                        (int)Enums.Roles.PERFORMER,
                        permissions.SelectedPerformResources);

                if (permissions.Administrator)
                    InsertPermission(ctx, employee.Id, (int)Enums.Roles.ADMINISTRATOR, 0);
            }
        }

        private void UpdateEmployee(PermissionsViewModel permissions)
        {
            var employee = Employee.GetEmployee(permissions.Id);
            SetSendMail(employee, permissions.SendMail);
            ActualizePermissions(employee, permissions.Author, (int)Enums.Roles.AUTHOR);
            ActualizePermissions(employee, permissions.Tester, (int)Enums.Roles.TESTER);
            ActualizePermissions(employee, permissions.SelectedApproveResources, (int)Enums.Roles.APPROVER);
            ActualizePermissions(employee, permissions.SelectedPerformResources, (int)Enums.Roles.PERFORMER);
            ActualizePermissions(employee, permissions.Administrator, (int)Enums.Roles.ADMINISTRATOR);
        }

        private void SetSendMail(Employee employee, bool sendMail)
        {
            using (var ctx = new SolutionsContext())
            {
                var emp = ctx.Employees.First(x => x.Id == employee.Id);
                emp.SendMail = sendMail;

                ctx.SaveChanges();
            }
        }

        private void ActualizePermissions(Employee employee, bool permissionEnabled, int roleId)
        {
            using (var ctx = new SolutionsContext())
            {
                if (permissionEnabled)
                {
                    if (!employee.HasRole(roleId))
                        InsertPermission(ctx, employee.Id, roleId, 0);
                }
                else
                {
                    if (employee.HasRole(roleId))
                    {
                        var permissionId = employee.Permissions.First(x =>
                            x.RoleId == roleId &&
                            x.EmployeeId == employee.Id).Id;

                        RemovePermission(ctx, permissionId);
                    }
                }
            }
        }

        private void ActualizePermissions(Employee employee, List<int> newResourceIds, int roleId)
        {
            var oldRolePermissions = employee.Permissions.Where(x => x.RoleId == roleId).ToList();

            using (var ctx = new SolutionsContext())
            {
                if (newResourceIds != null)
                {
                    foreach (var oldPermission in oldRolePermissions)
                    {
                        var permission = oldRolePermissions
                            .FirstOrDefault(x => newResourceIds.Contains(oldPermission.ResourceId));

                        if (permission == null)
                            RemovePermission(ctx, oldPermission.Id);
                    }

                    foreach (var newResourceId in newResourceIds)
                    {
                        if (oldRolePermissions != null)
                        {
                            var oldPermission = oldRolePermissions
                                .FirstOrDefault(x => x.ResourceId == newResourceId);

                            if (oldPermission == null)
                                InsertPermission(ctx, employee.Id, roleId, newResourceId);
                        }
                        else
                            InsertPermission(ctx, employee.Id, roleId, newResourceId);
                    }
                }
                else
                {
                    if (oldRolePermissions != null)
                    {
                        foreach (var permission in oldRolePermissions)
                            RemovePermission(ctx, permission.Id);
                    }
                }
            }
        }

        private void InsertPermissions(SolutionsContext ctx, int employeeId, int roleId, List<int> resourceIds)
        {
            foreach (var id in resourceIds)
                InsertPermission(ctx, employeeId, roleId, id);
        }

        private void InsertPermission(SolutionsContext ctx, int employeeId, int roleId, int resourceId)
        {
            var permission = new Permission()
            {
                EmployeeId = employeeId,
                RoleId = roleId,
                ResourceId = resourceId
            };

            ctx.Permissions.Add(permission);
            ctx.SaveChanges();
        }

        private void RemovePermission(SolutionsContext ctx, int permissionId)
        {
            var permission = ctx.Permissions.First(x => x.Id == permissionId);
            ctx.Permissions.Remove(permission);
            ctx.SaveChanges();
        }

        private List<Resource> BindPermissions(List<Permission> srcPermissions, int roleId)
        {
            var resources = new List<Resource>();
            var permissions = srcPermissions.Where(x => x.RoleId == roleId).ToList();
            permissions.ForEach(x => resources.Add(Resource.ById(x.ResourceId)));

            return resources;
        }

        private string AccountFormat(string account)
        {
            var length = account.Split('\\')[0].Length;
            return account.Substring(0, length).ToUpper() + account.Substring(length);
        }
    }
}