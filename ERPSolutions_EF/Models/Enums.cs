namespace ERPSolutions_EF.Models
{
    public class Enums
    {
        public enum Statuses
        {
            ON_APPROVE = 1,
            NOT_APPROVED,
            ON_PERFORM,
            DONE,
            HOLD,
            DECLINED,
            DELETED,
            CLOSED
        }

        public enum Roles
        {
            AUTHOR = 1,
            TESTER,
            APPROVER,
            PERFORMER,
            ADMINISTRATOR
        }

        public enum Resources
        {
            TEST = 1,
            PRODUCTION,
            DEVELOPMENT
        }

        public enum Priorities
        {
            A = 1,
            B,
            C
        }

        public enum CommentTypes
        {
            Approve = 1,
            Perform
        }
    }
}