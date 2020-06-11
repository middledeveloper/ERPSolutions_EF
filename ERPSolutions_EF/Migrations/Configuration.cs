namespace ERPSolutions_EF.Migrations
{
    using System.Data.Entity.Migrations;
    using ERPSolutions_EF.DAL;
    using ERPSolutions_EF.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<SolutionsContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SolutionsContext context)
        {
            context.CommentTypes.AddOrUpdate(
                new CommentType() { Title = "������������" },
                new CommentType() { Title = "����������" }
                );

            context.Operations.AddOrUpdate(
                new Operation() { Title = "����������" },
                new Operation() { Title = "��������������" },
                new Operation() { Title = "��������" }
                );

            context.Priorities.AddOrUpdate(
                new Priority() { Title = "A: ���������� ����������", Literal = "A" },
                new Priority() { Title = "B: � ���� ������������ | ��������� ������� ����", Literal = "B" },
                new Priority() { Title = "C: � �������� �������", Literal = "C" }
                );

            context.Resources.AddOrUpdate(
                new Resource() { Title = "ERP (�������� ������)" },
                new Resource() { Title = "ERP (������� ������)" },
                new Resource() { Title = "ERP (�������������� ������)" }
                );

            context.Roles.AddOrUpdate(
                new Role() { Title = "���������" },
                new Role() { Title = "�����������" },
                new Role() { Title = "�����������" },
                new Role() { Title = "�����������" },
                new Role() { Title = "�������������" }
                );

            context.Statuses.AddOrUpdate(
                new Status() { Title = "�� ������������" },
                new Status() { Title = "�� �����������" },
                new Status() { Title = "�� ����������" },
                new Status() { Title = "���������" },
                new Status() { Title = "�� �����" },
                new Status() { Title = "���������� ���������" },
                new Status() { Title = "�������" },
                new Status() { Title = "�������" }
                );

            context.Employees.AddOrUpdate(
                new Employee() { Account = "DOMAIN\\uz1", Active = true },
                new Employee() { Account = "DOMAIN\\uz2", Active = true },
                new Employee() { Account = "DOMAIN\\uz3", Active = true, SendMail = true },
                new Employee() { Account = "DOMAIN\\uz4", Active = false }
                );

            context.Permissions.AddOrUpdate(
                new Permission() { EmployeeId = 1, RoleId = 1 },
                new Permission() { EmployeeId = 1, RoleId = 2 },
                new Permission() { EmployeeId = 2, RoleId = 5 },
                new Permission() { EmployeeId = 3, RoleId = 4, ResourceId = 3 },
                new Permission() { EmployeeId = 4, RoleId = 1 },
                new Permission() { EmployeeId = 5, RoleId = 1 },
                new Permission() { EmployeeId = 5, RoleId = 2 },
                new Permission() { EmployeeId = 6, RoleId = 1 },
                new Permission() { EmployeeId = 6, RoleId = 2 },
                new Permission() { EmployeeId = 7, RoleId = 4, ResourceId = 1 },
                new Permission() { EmployeeId = 7, RoleId = 4, ResourceId = 2 },
                new Permission() { EmployeeId = 8, RoleId = 1 },
                new Permission() { EmployeeId = 8, RoleId = 1 },
                new Permission() { EmployeeId = 9, RoleId = 1 },
                new Permission() { EmployeeId = 9, RoleId = 2 },
                new Permission() { EmployeeId = 10, RoleId = 1 },
                new Permission() { EmployeeId = 10, RoleId = 2 },
                new Permission() { EmployeeId = 11, RoleId = 5 },
                new Permission() { EmployeeId = 12, RoleId = 1 },
                new Permission() { EmployeeId = 12, RoleId = 2 },
                new Permission() { EmployeeId = 13, RoleId = 1 },
                new Permission() { EmployeeId = 14, RoleId = 4, ResourceId = 1 },
                new Permission() { EmployeeId = 14, RoleId = 4, ResourceId = 2 },
                new Permission() { EmployeeId = 14, RoleId = 3, ResourceId = 2 },
                new Permission() { EmployeeId = 15, RoleId = 1 },
                new Permission() { EmployeeId = 15, RoleId = 2 },
                new Permission() { EmployeeId = 16, RoleId = 1 },
                new Permission() { EmployeeId = 16, RoleId = 2 },
                new Permission() { EmployeeId = 17, RoleId = 1 },
                new Permission() { EmployeeId = 17, RoleId = 2 },
                new Permission() { EmployeeId = 18, RoleId = 1 },
                new Permission() { EmployeeId = 18, RoleId = 2 },
                new Permission() { EmployeeId = 19, RoleId = 1 },
                new Permission() { EmployeeId = 19, RoleId = 2 },
                new Permission() { EmployeeId = 20, RoleId = 1 },
                new Permission() { EmployeeId = 20, RoleId = 2 },
                new Permission() { EmployeeId = 21, RoleId = 5 },
                new Permission() { EmployeeId = 22, RoleId = 1 },
                new Permission() { EmployeeId = 22, RoleId = 2 },
                new Permission() { EmployeeId = 23, RoleId = 1 },
                new Permission() { EmployeeId = 23, RoleId = 2 },
                new Permission() { EmployeeId = 24, RoleId = 1 },
                new Permission() { EmployeeId = 24, RoleId = 2 },
                new Permission() { EmployeeId = 25, RoleId = 1 },
                new Permission() { EmployeeId = 25, RoleId = 2 },
                new Permission() { EmployeeId = 26, RoleId = 5 }
                );
        }
    }
}
