namespace ERPSolutions_EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    TicketId = c.Int(nullable: false),
                    CommentTypeId = c.Int(nullable: false),
                    AuthorId = c.Int(nullable: false),
                    Date = c.DateTime(nullable: false),
                    Text = c.String(nullable: false, maxLength: 200),
                    Answer = c.String(maxLength: 200),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.AuthorId, cascadeDelete: false)
                .ForeignKey("dbo.CommentTypes", t => t.CommentTypeId, cascadeDelete: false)
                .Index(t => t.CommentTypeId)
                .Index(t => t.AuthorId);

            CreateTable(
                "dbo.Employees",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Account = c.String(nullable: false, maxLength: 50),
                    Active = c.Boolean(nullable: false),
                    SendMail = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.CommentTypes",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Title = c.String(nullable: false, maxLength: 50),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Operations",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Title = c.String(nullable: false, maxLength: 50),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Permissions",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    EmployeeId = c.Int(nullable: false),
                    RoleId = c.Int(nullable: false),
                    ResourceId = c.Int(),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Priorities",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Literal = c.String(nullable: false, maxLength: 1),
                    Title = c.String(nullable: false, maxLength: 50),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Resources",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Title = c.String(nullable: false, maxLength: 50),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Roles",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Title = c.String(nullable: false, maxLength: 50),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Statuses",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Title = c.String(nullable: false, maxLength: 50),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Tickets",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    ResourceId = c.Int(nullable: false),
                    AuthorId = c.Int(nullable: false),
                    Created = c.DateTime(nullable: false),
                    OperationId = c.Int(nullable: false),
                    Solutions = c.String(nullable: false, maxLength: 200),
                    TechnicalTask = c.String(maxLength: 200),
                    Desc = c.String(nullable: false, maxLength: 200),
                    FullDesc = c.String(maxLength: 1000),
                    Instructions = c.String(maxLength: 1000),
                    PriorityId = c.Int(nullable: false),
                    PriorityDesc = c.String(maxLength: 200),
                    TesterId = c.Int(),
                    Approved = c.DateTime(),
                    ApproverId = c.Int(),
                    Performed = c.DateTime(),
                    PerformerId = c.Int(),
                    StatusId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.ApproverId)
                .ForeignKey("dbo.Employees", t => t.AuthorId, cascadeDelete: false)
                .ForeignKey("dbo.Operations", t => t.OperationId, cascadeDelete: false)
                .ForeignKey("dbo.Employees", t => t.PerformerId)
                .ForeignKey("dbo.Priorities", t => t.PriorityId, cascadeDelete: false)
                .ForeignKey("dbo.Resources", t => t.ResourceId, cascadeDelete: false)
                .ForeignKey("dbo.Statuses", t => t.StatusId, cascadeDelete: false)
                .ForeignKey("dbo.Employees", t => t.TesterId)
                .Index(t => t.ResourceId)
                .Index(t => t.AuthorId)
                .Index(t => t.OperationId)
                .Index(t => t.PriorityId)
                .Index(t => t.TesterId)
                .Index(t => t.ApproverId)
                .Index(t => t.PerformerId)
                .Index(t => t.StatusId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.Tickets", "TesterId", "dbo.Employees");
            DropForeignKey("dbo.Tickets", "StatusId", "dbo.Statuses");
            DropForeignKey("dbo.Tickets", "ResourceId", "dbo.Resources");
            DropForeignKey("dbo.Tickets", "PriorityId", "dbo.Priorities");
            DropForeignKey("dbo.Tickets", "PerformerId", "dbo.Employees");
            DropForeignKey("dbo.Tickets", "OperationId", "dbo.Operations");
            DropForeignKey("dbo.Tickets", "AuthorId", "dbo.Employees");
            DropForeignKey("dbo.Tickets", "ApproverId", "dbo.Employees");
            DropForeignKey("dbo.Comments", "CommentTypeId", "dbo.CommentTypes");
            DropForeignKey("dbo.Comments", "AuthorId", "dbo.Employees");
            DropIndex("dbo.Tickets", new[] { "StatusId" });
            DropIndex("dbo.Tickets", new[] { "PerformerId" });
            DropIndex("dbo.Tickets", new[] { "ApproverId" });
            DropIndex("dbo.Tickets", new[] { "TesterId" });
            DropIndex("dbo.Tickets", new[] { "PriorityId" });
            DropIndex("dbo.Tickets", new[] { "OperationId" });
            DropIndex("dbo.Tickets", new[] { "AuthorId" });
            DropIndex("dbo.Tickets", new[] { "ResourceId" });
            DropIndex("dbo.Comments", new[] { "AuthorId" });
            DropIndex("dbo.Comments", new[] { "CommentTypeId" });
            DropTable("dbo.Tickets");
            DropTable("dbo.Statuses");
            DropTable("dbo.Roles");
            DropTable("dbo.Resources");
            DropTable("dbo.Priorities");
            DropTable("dbo.Permissions");
            DropTable("dbo.Operations");
            DropTable("dbo.CommentTypes");
            DropTable("dbo.Employees");
            DropTable("dbo.Comments");
        }
    }
}
