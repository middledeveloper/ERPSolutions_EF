using System.Data.Entity.ModelConfiguration;
using ERPSolutions_EF.Models;

namespace ERPSolutions_EF.EntityConfigurations
{
    public class CommentConfiguration : EntityTypeConfiguration<Comment>
    {
        public CommentConfiguration()
        {
            ToTable("Comments");

            Property(c => c.Id)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);

            Property(c => c.TicketId)
                .IsRequired();

            Property(c => c.CommentTypeId)
                .IsRequired();

            Property(c => c.Text)
                .HasMaxLength(200)
                .IsRequired();

            Property(c => c.Answer)
                .HasMaxLength(200);

            Property(c => c.Date)
                .IsRequired();
        }
    }
}