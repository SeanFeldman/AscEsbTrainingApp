using System;
using NServiceBus;
// Written by JPratt 2012

namespace AscEsbTrainingSubConsole
{
    class EndpointConfig : IConfigureThisEndpoint, AsA_Server, IWantCustomInitialization
    {
        public void Init()
        {
            Console.Title = "AscEsbTrainingSubConsole (SUBSCRIBER)";

            Configure.With()
                //this overrides the NServiceBus default convention of IEvent 
                .DefiningEventsAs(t => t.Namespace != null && t.Namespace.StartsWith("AscEsbTrainingMessages"))
                .DisableTimeoutManager(); // Stops default creation of RavenDB database.
        }
    }
}
