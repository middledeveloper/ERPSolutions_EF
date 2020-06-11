using System.Data.Entity.ModelConfiguration;
using ERPSolutions_EF.Models;

namespace ERPSolutions_EF.EntityConfigurations
{
    public class CommentTypeConfiguration : EntityTypeConfiguration<CommentType>
    {
        public CommentTypeConfiguration()
        {
            ToTable("CommentTypes");

            Property(ct => ct.Id)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);

            Property(ct => ct.Title)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}