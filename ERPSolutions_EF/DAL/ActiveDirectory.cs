using System;
using System.DirectoryServices;
using System.Linq;
using System.Web.Hosting;

namespace ERPSolutions_EF.DAL
{
    public class ActiveDirectory
    {
        private static readonly string DirectoryEntry = "LDAP://DC=DOMAIN,DC=ru";
        private static readonly string DirectoryObjectFormat = "(&(objectCategory=person)(sAMAccountName={0}))";
        private static readonly string CnProperty = "cn";
        private static readonly string MailProperty = "mail";

        private static readonly char BackslashChar = '\\';

        public static string GetName(string account)
        {

            using (HostingEnvironment.Impersonate())
            {
                var entry = new DirectoryEntry(DirectoryEntry);
                var search = new DirectorySearcher(entry)
                {
                    Filter = string.Format(DirectoryObjectFormat, account.Split(BackslashChar).Last())
                };

                search.PropertiesToLoad.Add(CnProperty);
                try
                {
                    var result = search.FindOne();
                    return (result.Properties[CnProperty][0].ToString());
                }
                catch (Exception)
                {
                    return account;
                }
            }
        }

        public static string GetEmail(string account)
        {
            using (HostingEnvironment.Impersonate())
            {
                var entry = new DirectoryEntry(DirectoryEntry);
                var search = new DirectorySearcher(entry)
                {
                    Filter = string.Format(DirectoryObjectFormat, account.Split(BackslashChar).Last())
                };

                search.PropertiesToLoad.Add(MailProperty);
                try
                {
                    var result = search.FindOne();
                    return result.Properties[MailProperty][0].ToString();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}