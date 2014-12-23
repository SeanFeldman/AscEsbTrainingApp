using System;
using System.Threading;
using AscEsbTrainingMessages;
using NServiceBus;

namespace AscEsbTrainingSubConsole
{
    public class EventMessageHandler : IHandleMessages<EventMessageConsole>, IHandleMessages<EventMessageUI>
    {
        void GeneralHandle(IMyEvent message)
        {
            // Display
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\n[{0}] from {1}", DateTime.Now.ToString(("MM/dd/yyyy hh:mm:ss.fff")), message.Publisher);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("ID:      {0}", message.ID);
            Console.WriteLine("Message: {0}.", message.Message);

            // Process
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Processing");

            var interval = new TimeSpan(0, 0, 0, 0, 250); // 250 milliseconds
            var rand = new Random();
            var times = rand.Next(2, 12);
            for (var i = 0; i < times; i++)
            {
                Thread.Sleep(interval); // Delay
                Console.Write(".");
            }
            Console.WriteLine("   Done!");
        }

        public void Handle(EventMessageConsole message)
        {
            GeneralHandle(message);
        }

        public void Handle(EventMessageUI message)
        {
            GeneralHandle(message);
        }
    }
}
