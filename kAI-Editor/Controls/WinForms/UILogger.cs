using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;


using kAI.Core;
using kAI.Editor.Core;

namespace kAI.Editor.Controls.WinForms
{
    /// <summary>
    /// A logger with a command entry line at the bottom.
    /// </summary>
    public partial class UILogger : UserControl, kAIILogger
    {
        const string kDefaultString = " > [enter command or Help() for a list, enter to run]";
               
        /// <summary>
        /// Create a new logger.
        /// </summary>
        public UILogger()
        {
            InitializeComponent();            
        }

        /// <summary>
        /// Log a standard message. 
        /// </summary>
        /// <param name="lMessage">The message</param>
        /// <param name="lDetails">Optionally, additional information. </param>
        public void LogMessage(string lMessage, params KeyValuePair<string, object>[] lDetails)
        {
            AddString(FormatMessage(lMessage));
            foreach (KeyValuePair<string, object> lDetail in lDetails)
            {
                AddString(FormatDetail(lDetail));
            }
        }

        /// <summary>
        /// Log a warning.
        /// </summary>
        /// <param name="lWarning">The warning. </param>
        /// <param name="lDetails">Optionally, aditional information. </param>
        public void LogWarning(string lWarning, params KeyValuePair<string, object>[] lDetails)
        {
            AddString(FormatMessage("Warning: " + lWarning));
            foreach (KeyValuePair<string, object> lDetail in lDetails)
            {
                AddString(FormatDetail(lDetail));
            }
        }

        /// <summary>
        /// Log a error. 
        /// </summary>
        /// <param name="lError">The error. </param>
        /// <param name="lDetails">Optionally, aditional information. </param>
        public void LogError(string lError, params KeyValuePair<string, object>[] lDetails)
        {
            AddString(FormatMessage("ERROR: " + lError));
            foreach (KeyValuePair<string, object> lDetail in lDetails)
            {
                AddString(FormatDetail(lDetail));
            }
        }


        /// <summary>
        /// Log a critical error. 
        /// </summary>
        /// <param name="lError">The error. </param>
        /// <param name="lDetails">Optionally, aditional information. </param>
        public void LogCriticalError(string lError, params KeyValuePair<string, object>[] lDetails)
        {
            AddString(FormatMessage("ERROR: " + lError));
            foreach (KeyValuePair<string, object> lDetail in lDetails)
            {
                AddString(FormatDetail(lDetail));
            }
        }

        /// <summary>
        /// Format the string for outputting, just appends a clock at the moment. 
        /// </summary>
        /// <param name="lMessage">The message to append a clock to. </param>
        /// <returns>The formatted string. </returns>
        private string FormatMessage(string lMessage)
        {
            return DateTime.Now.ToString("HH:mm:ss") + ": " + lMessage;
        }

        /// <summary>
        /// Format an indiviual detail. 
        /// </summary>
        /// <param name="lDetail">The KV-Pair representing the detail. </param>
        /// <returns>The fomatted string. </returns>
        private string FormatDetail(KeyValuePair<string, object> lDetail)
        {
            // We add a space for the time stamp that is printed in the main message
            return "        \t" + lDetail.Key + ": " + lDetail.Value.ToString();
        }

        /// <summary>
        /// Add the string to the output, forcing the window to scroll to the bottom if required. 
        /// </summary>
        /// <param name="lString">The string to add. </param>
        private void AddString(string lString)
        {
            mMessageBox.Items.Add(lString);

            int lNumberOfVisibleItems = mMessageBox.ClientSize.Height / mMessageBox.ItemHeight;

            // Put the top item to be the last - number of visible 
            mMessageBox.TopIndex = Math.Max(mMessageBox.Items.Count - lNumberOfVisibleItems + 1, 0);
        }

        private void mCmdTextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r' || e.KeyChar == '\n')
            {
                if(mCmdTextbox.Text != kDefaultString)
                {
                    AddString(FormatMessage(mCmdTextbox.Text));
                    bool lResult = kAIInteractionTerminal.RunCommand(mCmdTextbox.Text);

                    if (lResult)
                    {
                        mCmdTextbox.Text = "";
                    }
                }
            }
        }

        private void mCmdTextbox_Enter(object sender, EventArgs e)
        {
            if (mCmdTextbox.Text == kDefaultString)
            {
                mCmdTextbox.Text = "";
            }
        }

        private void mCmdTextbox_Leave(object sender, EventArgs e)
        {
            if (mCmdTextbox.Text == "")
            {
                mCmdTextbox.Text = kDefaultString;
            }
        }
    }
}
