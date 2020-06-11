using System;
using System.Linq;
using System.ServiceProcess;
using System.Timers;
using ERPSolutions_EF.DAL;
using ERPSolutions_EF.Models;

namespace TicketReminder
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
            var title = "InstallSolutions";

            if (!System.Diagnostics.EventLog.SourceExists(title))
                System.Diagnostics.EventLog.CreateEventSource(title, title + " LOG");

            eventLog1.Source = title;
            eventLog1.Log = title + " LOG";

            var timer = new Timer();
            timer.Start();
            timer.Interval = 60_000;
            timer.Elapsed += Timer_elapsed;
        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("Служба Windows Журнала установки решений ЗАПУЩЕНА");
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("Служба Windows Журнала установки решений ОСТАНОВЛЕНА");
        }

        protected void Timer_elapsed(object sender, ElapsedEventArgs e)
        {
            if (new[] { 0, 30 }.Contains(DateTime.Now.Minute))
            {
                PriorityReminder();

                if (DateTime.Now.Minute == 0 && DateTime.Now.Hour == 9)
                    DailyReminder();
            }
        }

        protected void PriorityReminder()
        {
            eventLog1.WriteEntry("Отправка оповещений о заявках класса 'А'");

            var activePriorityATickets = Ticket.ActivePriorityRepo((int)Enums.Priorities.A);

            if (activePriorityATickets.Count() > 0)
                activePriorityATickets.ForEach(x => Mail.Send(x));

            eventLog1.WriteEntry("Отправка оповещений о заявках класса 'А' завершена (" +
                activePriorityATickets.Count() + " шт.)");
        }

        protected void DailyReminder()
        {
            eventLog1.WriteEntry("Ежесуточная отправка оповещений о заявках");
            var activeTickets = Ticket.ActiveRepo();

            if (activeTickets.Count() > 0)
                activeTickets.ForEach(x => Mail.Send(x));

            eventLog1.WriteEntry("Ежесуточная отправка оповещений завершена (" +
                activeTickets.Count() + " шт.)");
        }
    }
}