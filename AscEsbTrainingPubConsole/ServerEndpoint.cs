using System;
using System.Reflection;
using System.Threading;
using AscEsbTrainingMessages;
using log4net;
using log4net.Config;
using NServiceBus;

// Written by JPratt 2012

namespace AscEsbTrainingPubConsole
{
    public class ServerEndpoint : IWantToRunWhenBusStartsAndStops, IWantCustomInitialization
    {
        public IBus Bus { get; set; }

        private static readonly ILog Logger = LogManager.GetLogger("Name");

        public void Start()
        {
            XmlConfigurator.Configure();

            Logger.Debug("Here is a debug log.");
            Logger.Info("... and an Info log.");
            Logger.Warn("... and a warning.");
            Logger.Error("... and an error.");
            Logger.Fatal("... and a fatal error.");

            Console.Title = "AscEsbTrainingPubConsole (PUBLISHER)";

            var msgCount = 0;
            var rand = new Random();

            // Infinite loop
            while (true)
            {
                // Defaults
                var number = 0;
                var noDelay = false;

                Thread.Sleep(5000);

                // Get input from user
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Enter a message to publish to the subscriber(s).");
                Console.WriteLine("Enter a number to publish the same number of random $ values.");
                Console.Write(">> ");

                Console.ForegroundColor = ConsoleColor.White;
                var input = Console.ReadLine();

                // Parse the user input
                if (input != null)
                {
                    var words = input.Split(' ');
                    if (words.Length > 0) int.TryParse(words[0], out number);       // Is the first word a number?
                    if (words.Length > 1) noDelay = words[1].ToLower() == "n";      // Is the second word a letter 'n'?
                }

                // Send out the same number of messages
                if (number > 0)
                {
                    // Record the start time
                    var startTime = DateTime.Now;

                    // Process
                    for (var i = 0; i < number; i++)
                    {
                        // Generate a random number
                        var amount = Math.Round(rand.NextDouble() * 1000000.0, 2);

                        // Publish a message to notify UI
                        PublishMessage("$" + amount, msgCount++);

                        // Delay 1.5 second to insert next record
                        if (!noDelay) Thread.Sleep(1500);
                    }

                    // Calculate performance
                    var span = DateTime.Now - startTime;
                    if ((number > 1) && (span.TotalMilliseconds != 0))
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("\nPerformance: {0} messages per second", number * 1000 / span.TotalMilliseconds);
                    }
                }
                // Otherwise, just send out a message
                else
                {
                    PublishMessage(input, msgCount++);
                }
            }
        }

        public void Stop()
        {

        }

        public void Init()
        {
            Configure.With()
                //this overrides the NServiceBus default convention of IEvent 
                .MsmqSubscriptionStorage() // Stores subscription info in an MSMQ instead of RavenDB (default).
                .DisableTimeoutManager();  // Stops default creation of RavenDB database.
        }


        /// <summary>
        /// Publish a mesage
        /// </summary>
        private void PublishMessage(string message, int id)
        {
            // Publish
            var eventMessage = Bus.CreateInstance<EventMessageConsole>();// : new EventMessage();
            eventMessage.EventId = Guid.NewGuid();
            eventMessage.Time = DateTime.Now.Second > 30 ? (DateTime?)DateTime.Now : null;
            eventMessage.Duration = TimeSpan.FromSeconds(99999D);
            eventMessage.ID = id;
            eventMessage.Message = message;
            eventMessage.Publisher = Assembly.GetExecutingAssembly().GetName().Name + "@" + Environment.MachineName;
            
            Bus.Publish(eventMessage);

            // Display
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\n[{0} Sent]", DateTime.Now.ToString(("MM/dd/yyyy hh:mm:ss.fff")));
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("ID:      {0}", eventMessage.ID);
            Console.WriteLine("Message: {0}.", eventMessage.Message);
        }
    }
}
