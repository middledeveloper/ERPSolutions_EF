using System.Data.Entity.ModelConfiguration;
using ERPSolutions_EF.Models;

namespace ERPSolutions_EF.EntityConfigurations
{
    public class PriorityConfiguration : EntityTypeConfiguration<Priority>
    {
        public PriorityConfiguration()
        {
            ToTable("Priorities");

            Property(p => p.Id)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);

            Property(p => p.Literal)
                .HasMaxLength(1)
                .IsRequired();

            Property(p => p.Title)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}