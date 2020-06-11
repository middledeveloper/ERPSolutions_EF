using System.Data.Entity.ModelConfiguration;
using ERPSolutions_EF.Models;

namespace ERPSolutions_EF.EntityConfigurations
{
    public class ResourceConfiguration : EntityTypeConfiguration<Resource>
    {
        public ResourceConfiguration()
        {
            ToTable("Resources");

            Property(s => s.Id)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);

            Property(s => s.Title)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}