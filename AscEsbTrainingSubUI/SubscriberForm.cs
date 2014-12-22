using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using AscEsbTrainingMessages;
using NServiceBus;
using NServiceBus.Config;

namespace AscEsbTrainingSubUI
{
    public partial class SubscriberForm : Form //, IWantCustomInitialization
    {
        public IBus Bus { get; set; }
        delegate void UpdateControlsCallback(IMyEvent message);

        public SubscriberForm()
        {
            InitializeComponent();

            // Add a link to the LinkLabel.
            LinkLabel.Link link = new LinkLabel.Link();
            link.LinkData = "http://ittfs01/sites/ASC%20Team%20Project/EnterpriseArchitecture/ASC%20Published%20Messages/NServiceBus%20Training%20App%20Developers%20Guide.docx";
            linkLabel1.Links.Add(link);

        }

        public void UpdateControls(IMyEvent message)
        {
            if (InvokeRequired)
            {
                var d = new UpdateControlsCallback(UpdateControls);
                Invoke(d, new object[] { message });
            }
            else
            {
                richTextBoxReceivedMessages.DeselectAll();
                richTextBoxReceivedMessages.SelectionFont = new Font("Letter Gothic", 9f, FontStyle.Bold);
                richTextBoxReceivedMessages.AppendText(string.Format("[{0}] ", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff")));
                richTextBoxReceivedMessages.SelectionFont = new Font(richTextBoxReceivedMessages.SelectionFont, FontStyle.Italic);
                richTextBoxReceivedMessages.AppendText(" from " + message.Publisher);

                richTextBoxReceivedMessages.SelectionFont = new Font(richTextBoxReceivedMessages.SelectionFont, FontStyle.Bold);
                richTextBoxReceivedMessages.AppendText("\r\n  ID:\t\t");
                richTextBoxReceivedMessages.SelectionFont = new Font(richTextBoxReceivedMessages.SelectionFont, FontStyle.Italic);
                richTextBoxReceivedMessages.AppendText(message.ID.ToString(CultureInfo.InvariantCulture));

                richTextBoxReceivedMessages.SelectionFont = new Font(richTextBoxReceivedMessages.SelectionFont, FontStyle.Bold);
                richTextBoxReceivedMessages.AppendText("\r\n  Message:\t");
                richTextBoxReceivedMessages.SelectionFont = new Font(richTextBoxReceivedMessages.SelectionFont, FontStyle.Italic);
                richTextBoxReceivedMessages.AppendText(message.Message + "\r\n\r\n");

                richTextBoxReceivedMessages.SelectionStart = richTextBoxReceivedMessages.Text.Length;
                richTextBoxReceivedMessages.ScrollToCaret();
            }
        }


        private void SubscriberFormShown(object sender, EventArgs e)
        {
            var purgeQueue = false;

            if (bool.Parse(ConfigurationManager.AppSettings["PromptQueuePurge"]))
            {
                purgeQueue =
                    (MessageBox.Show(
                        "Purge existing messages from queue? The message queue may have accumulated messages while this subscriber application was off. Click 'Yes' to clear the queue and start fresh. Click 'No' to view all messages that may have already be queued.",
                        "Purge Message Queue?",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.Yes);
            }

            try
            {
                Refresh();
                Thread.Sleep(1000);

                labelStatus.ForeColor = Color.Olive;
                labelStatus.Text = "Preparing to receive messages from publisher(s)...";

                

                Bus = Configure
                        .With(/*AllAssemblies.Except("setup.exe")*/)
                        .DefaultBuilder()
                        .DefiningEventsAs(t => t.Namespace != null && t.Namespace.StartsWith("AscEsbTrainingMessages"))
                        .DisableTimeoutManager() // Stops default creation of RavenDB database.
                        .UseTransport<Msmq>()    // Configre to use MSMQ
                        .PurgeOnStartup(purgeQueue) // Purge existing messages in the subscription queue.
                        .UnicastBus()
                        .LoadMessageHandlers()
                        .RunHandlersUnderIncomingPrincipal(false)
                        .CreateBus()
                        .Start(() => Configure.Instance.ForInstallationOn<NServiceBus.Installation.Environments.Windows>().Install());
                
                //Bus.Subscribe(As);


                labelStatus.ForeColor = Color.Green;
                labelStatus.Text = "Ready to receive publisher messages.";
            }
            catch (Exception ex)
            {
                // Show contents of exception on the GUI
                labelStatus.ForeColor = Color.Red;
                labelStatus.Text = String.Format(ex.ToString());
            }
        }

        private void ButtonClearReceivedMessagesClick(object sender, EventArgs e)
        {
            richTextBoxReceivedMessages.Clear();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Send the URL to the operating system.
            Process.Start(e.Link.LinkData as string);
        }
    }

    public class LoadMessageHandler : IHandleMessages<IMyEvent>
    {
        public IBus Bus { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        public void Handle(IMyEvent message)
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form is SubscriberForm)
                {
                    var oForm = form as SubscriberForm;
                    oForm.UpdateControls(message);
                }
            }
        }
    }
}


