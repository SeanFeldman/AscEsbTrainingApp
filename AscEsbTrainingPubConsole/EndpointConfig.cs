using NServiceBus;

// Written by JPratt 2012
namespace AscEsbTrainingPubConsole
{
    class EndpointConfig : IConfigureThisEndpoint, AsA_Publisher, IWantCustomInitialization
    {
        public void Init()
        {
            Configure.Transactions.Enable();
            Configure.With(AllAssemblies.Matching("AscEsbTrainingMessages"))
                .DefaultBuilder()
                //this overrides the NServiceBus default convention of IEvent
                .DefiningEventsAs(t => t.Namespace != null && t.Namespace.StartsWith("AscEsbTrainingMessages"));
        }
    }
}
