using System.Data.Entity.ModelConfiguration;
using ERPSolutions_EF.Models;

namespace ERPSolutions_EF.EntityConfigurations
{
    public class RoleConfiguration : EntityTypeConfiguration<Role>
    {
        public RoleConfiguration()
        {
            ToTable("Roles");

            Property(r => r.Id)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);

            Property(r => r.Title)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}