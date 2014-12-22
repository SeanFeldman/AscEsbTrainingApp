using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using AscEsbTrainingMessages;
using NServiceBus;

namespace AscEsbTrainingPubUI
{
    public partial class PublisherForm : Form
    {
        public IBus Bus { get; set; }
        private int msgCount = 0;
        public PublisherForm()
        {
            InitializeComponent();

            // Add a link to the LinkLabel.
            var link = new LinkLabel.Link();
            link.LinkData = "http://ittfs01/sites/ASC%20Team%20Project/EnterpriseArchitecture/ASC%20Published%20Messages/NServiceBus%20Training%20App%20Developers%20Guide.docx";
            linkLabel1.Links.Add(link);
        }

        private void ButtonPublishMessageClick(object sender, EventArgs e)
        {


            ProcessMessage(textBoxMessage.Text, msgCount);
        }


        private void ProcessMessage(string message, int id)
        {
            int number = 0;

            int.TryParse(message, out number);

            var rand = new Random();

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
                    PublishMessage("$" + amount, msgCount);

                    // Delay 1.5 second to insert next record
                    Thread.Sleep(1500);
                }

            }
            else
            {
                PublishMessage(message, id);
            }
            
        }

        /// <summary>
        /// Publish a mesage
        /// </summary>
        private void PublishMessage(string message, int id)
        {
            // Publish
            try
            {
                var eventMessage = Bus.CreateInstance<IMyEvent>();// : new EventMessage();
                eventMessage.EventId = Guid.NewGuid();
                eventMessage.Time = DateTime.Now.Second > 30 ? (DateTime?)DateTime.Now : null;
                eventMessage.Duration = TimeSpan.FromSeconds(99999D);
                eventMessage.ID = id;
                eventMessage.Message = message;
                eventMessage.Publisher = Assembly.GetExecutingAssembly().GetName().Name + "@" + Environment.MachineName;

                Bus.Publish(eventMessage);

                textBoxMessage.Clear();

                labelStatus.ForeColor = Color.Green;
                labelStatus.Text = "Published message: \"" + message + "\"";
                Refresh();
            }
            catch (Exception ex)
            {
                labelStatus.ForeColor = Color.Red;
                labelStatus.Text = ex.Message;
                throw;
            }

            msgCount++;
        }

        private void PublisherFormShown(object sender, EventArgs e)
        {
            try
            {
                buttonPublishMessage.Enabled = false;
                Refresh();
                Thread.Sleep(1000);
                labelStatus.ForeColor = Color.Olive;
                labelStatus.Text = "Preparing publisher...";
                Refresh();

                Thread.Sleep(1000);
                
                // Sets up the Bus to receive messages from publisher endpoints. See config file section
                Configure.Transactions.Enable();
                Bus = Configure
                        .With(AllAssemblies.Matching("AscEsbTrainingMessages"))
                        .DefiningEventsAs(t => t.Namespace != null && t.Namespace.StartsWith("AscEsbTrainingMessages"))
                        .Log4Net()
                        .DefaultBuilder()
                        //.IsTransactional(true)
                        .DisableTimeoutManager() // Stops default creation of RavenDB database.
                        .UseTransport<Msmq>()
                        .MsmqSubscriptionStorage()
                        .UnicastBus()
                        .CreateBus()
                        .Start(() => Configure.Instance.ForInstallationOn<NServiceBus.Installation.Environments.Windows>().Install());
                

                labelStatus.ForeColor = Color.Green;
                labelStatus.Text = "Ready to publish messages.";
                buttonPublishMessage.Enabled = true;
            }
            catch (Exception ex)
            {
                // Show contents of exception on the GUI
                labelStatus.Text = String.Format(ex.ToString());
            }
        }

        private void TextBoxMessageKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ProcessMessage(textBoxMessage.Text, msgCount);
                e.Handled = true;
            }
        }
    }
}
