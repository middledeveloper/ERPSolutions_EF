using System.Data.Entity.ModelConfiguration;
using ERPSolutions_EF.Models;

namespace ERPSolutions_EF.EntityConfigurations
{
    public class OperationConfiguration : EntityTypeConfiguration<Operation>
    {
        public OperationConfiguration()
        {
            ToTable("Operations");

            Property(a => a.Id)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);

            Property(a => a.Title)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}