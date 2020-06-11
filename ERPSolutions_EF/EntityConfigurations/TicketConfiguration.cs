using System.Data.Entity.ModelConfiguration;
using ERPSolutions_EF.Models;

namespace ERPSolutions_EF.EntityConfigurations
{
    public class TicketConfiguration : EntityTypeConfiguration<Ticket>
    {
        public TicketConfiguration()
        {
            ToTable("Tickets");

            Property(r => r.Id)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);

            Property(r => r.OperationId)
                .IsRequired();

            Property(r => r.AuthorId)
                .IsRequired();

            Property(r => r.Created)
                .IsRequired();

            Property(r => r.PriorityId)
                .IsRequired();

            Property(r => r.Solutions)
                .HasMaxLength(200)
                .IsRequired();

            Property(r => r.TechnicalTask)
               .HasMaxLength(200);

            Property(r => r.Desc)
               .HasMaxLength(200);

            Property(r => r.FullDesc)
               .HasMaxLength(1000);

            Property(r => r.Instructions)
               .HasMaxLength(1000);

            Property(r => r.PriorityDesc)
               .HasMaxLength(200);

            Property(r => r.StatusId)
                .IsRequired();
        }
    }
}