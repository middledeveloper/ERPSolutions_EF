using System.Data.Entity.ModelConfiguration;
using ERPSolutions_EF.Models;

namespace ERPSolutions_EF.EntityConfigurations
{
    public class PermissionConfiguration : EntityTypeConfiguration<Permission>
    {
        public PermissionConfiguration()
        {
            ToTable("Permissions");

            Property(u => u.Id)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);

            Property(ur => ur.EmployeeId)
                .IsRequired();

            Property(ur => ur.RoleId)
                .IsRequired();

            Property(ur => ur.ResourceId)
                .IsOptional();
        }
    }
}