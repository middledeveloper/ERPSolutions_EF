using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ERPSolutions_EF.DAL;

namespace ERPSolutions_EF.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public CommentType CommentType { get; set; }
        public int CommentTypeId { get; set; }
        public Employee Author { get; set; }
        public int AuthorId { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public string Answer { get; set; }

        public static List<Comment> ByType(int ticketId, int commentTypeId)
        {
            using (var ctx = new SolutionsContext())
            {
                var comments = ctx.Comments.Where(x =>
                    x.TicketId == ticketId &&
                    x.CommentTypeId == commentTypeId)
                    .AsNoTracking()
                    .ToList();

                comments.ForEach(x => x.Author = Employee.GetEmployee(x.AuthorId));

                return comments;
            }
        }

        public static void Save(int ticketId, int employeeId, string text, int commentType)
        {
            using (var ctx = new SolutionsContext())
            {
                var comment = new Comment()
                {
                    AuthorId = employeeId,
                    CommentTypeId = commentType,
                    Date = DateTime.Now,
                    TicketId = ticketId,
                    Text = text
                };

                ctx.Comments.Add(comment);
                ctx.SaveChanges();
            }
        }

        public static void Update(int commentId, string answer)
        {
            using (var ctx = new SolutionsContext())
            {
                var comment = ctx.Comments.First(x => x.Id == commentId);
                comment.Answer = answer;

                ctx.SaveChanges();
            }
        }
    }
}