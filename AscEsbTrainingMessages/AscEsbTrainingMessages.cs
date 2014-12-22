using System;

// Written by JPratt 2012
// This class contains the details of the message that will be published to the ESB.
namespace AscEsbTrainingMessages
{
        [Serializable]
        public class EventMessageConsole : IMyEvent
        {
            public Guid EventId { get; set; }
            public DateTime? Time { get; set; }
            public TimeSpan Duration { get; set; }
            public int ID { get; set; }           // What number message this is
            public string Message { get; set; }   // Message the user types on the publisher Console UI
            public string Publisher { get; set; }
        }

        [Serializable]
        public class EventMessageUI : IMyEvent
        {
            public Guid EventId { get; set; }
            public DateTime? Time { get; set; }
            public TimeSpan Duration { get; set; }
            public int ID { get; set; }           // What number message this is
            public string Message { get; set; }   // Message the user types on the publisher Console UI
            public string Publisher { get; set; }
        }


        public interface IMyEvent
        {
            Guid EventId { get; set; }
            DateTime? Time { get; set; }
            TimeSpan Duration { get; set; }
            string Publisher { get; set; }
            int ID { get; set; }
            string Message { get; set; }
        }
}

