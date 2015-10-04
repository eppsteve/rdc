/*--------------------------------------------------------------------- 
 * Remote Desktop Control
 * -------------------------
 * Developed by Steve Alogaris
 * 
 * The application establishes a connection to a remote host allowing
 * remote control by sending mouse and keyboards commands. The desktop
 * can be viewed remotely.
 *----------------------------------------------------------------------*/

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Client
{
    public partial class Client : Form
    {
        private TcpClient connector;
        public String ipAdress;
        public int port = 1818;

        private Stream stream;
        private StreamWriter eventSender;
        private Thread theThread;
        private int resolutionX;
        private int resolutionY;
        public bool sendKeysAndMouse = false;
        private bool connected = false;

        private frmConnectDialog connectDialog;
        private AboutBox1 aboutDialog;
        private frmUpdateInterval updateDialog;

        public Client()
        {
            InitializeComponent();
        }

        public void startConnect()
        {
            try
            {
                connector = new TcpClient(ipAdress, port);
                connected = true;
                statusLabel.Text = "Connection Established with " + ipAdress;

                theThread = new Thread(new ThreadStart(startRead));
                theThread.Start();
            }
            catch (Exception problem)
            {
                MessageBox.Show("Cannot establish a connection", "Failure!");
                //showConnect();
            }
        }

        public void imageDelayChange(int x)
        {
            eventSender.Write("CDELAY " + x + "\n");
            eventSender.Flush();
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            //if (theThread.IsAlive)
            //    theThread.Abort();
        }

        //gets the image sent by the server
        private void startRead()
        {
            try
            {
                stream = connector.GetStream();
                eventSender = new StreamWriter(stream);
                while (true)
                {
                    BinaryFormatter bFormat = new BinaryFormatter();
                    Bitmap inImage = bFormat.Deserialize(stream) as Bitmap;
                    resolutionX = inImage.Width;
                    resolutionY = inImage.Height;
                    theImage.Image = (Image) inImage;
                }
            }
            catch (Exception) {}
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        public void sendShutdown()
        {
            eventSender.Write("SHUTDOWN\n");
            eventSender.Flush();
            //this.Close();
            statusLabel.Text = "Server terminated. Connection has been lost.";
        }

        //sends keyboard commands to the server
        private void Form2_KeyUp(object sender, KeyEventArgs e)
        {
            if (!sendKeysAndMouse)
                return;
            try
            {
                String keysToSend = "";
                if (e.Shift)
                    keysToSend += "+";
                if (e.Alt)
                    keysToSend += "%";
                if (e.Control)
                    keysToSend += "^";

                if (e.KeyValue >= 65 && e.KeyValue <= 90)
                    keysToSend += e.KeyCode.ToString().ToLower();
                else if (e.KeyCode.ToString().Equals("Back"))
                    keysToSend += "{BS}";
                else if (e.KeyCode.ToString().Equals("Pause"))
                    keysToSend += "{BREAK}";
                else if (e.KeyCode.ToString().Equals("Capital"))
                    keysToSend += "{CAPSLOCK}";
                else if (e.KeyCode.ToString().Equals("Space"))
                    keysToSend += " ";
                else if (e.KeyCode.ToString().Equals("Home"))
                    keysToSend += "{HOME}";
                else if (e.KeyCode.ToString().Equals("Return"))
                    keysToSend += "{ENTER}";
                else if (e.KeyCode.ToString().Equals("End"))
                    keysToSend += "{END}";
                else if (e.KeyCode.ToString().Equals("Tab"))
                    keysToSend += "{TAB}";
                else if (e.KeyCode.ToString().Equals("Escape"))
                    keysToSend += "{ESC}";
                else if (e.KeyCode.ToString().Equals("Insert"))
                    keysToSend += "{INS}";
                else if (e.KeyCode.ToString().Equals("Up"))
                    keysToSend += "{UP}";
                else if (e.KeyCode.ToString().Equals("Down"))
                    keysToSend += "{DOWN}";
                else if (e.KeyCode.ToString().Equals("Left"))
                    keysToSend += "{LEFT}";
                else if (e.KeyCode.ToString().Equals("Right"))
                    keysToSend += "{RIGHT}";
                else if (e.KeyCode.ToString().Equals("PageUp"))
                    keysToSend += "{PGUP}";
                else if (e.KeyCode.ToString().Equals("Next"))
                    keysToSend += "{PGDN}";
                else if (e.KeyCode.ToString().Equals("Tab"))
                    keysToSend += "{TAB}";
                else if (e.KeyCode.ToString().Equals("D1"))
                    keysToSend += "1";
                else if (e.KeyCode.ToString().Equals("D2"))
                    keysToSend += "2";
                else if (e.KeyCode.ToString().Equals("D3"))
                    keysToSend += "3";
                else if (e.KeyCode.ToString().Equals("D4"))
                    keysToSend += "4";
                else if (e.KeyCode.ToString().Equals("D5"))
                    keysToSend += "5";
                else if (e.KeyCode.ToString().Equals("D6"))
                    keysToSend += "6";
                else if (e.KeyCode.ToString().Equals("D7"))
                    keysToSend += "7";
                else if (e.KeyCode.ToString().Equals("D8"))
                    keysToSend += "8";
                else if (e.KeyCode.ToString().Equals("D9"))
                    keysToSend += "9";
                else if (e.KeyCode.ToString().Equals("D0"))
                    keysToSend += "0";
                else if (e.KeyCode.ToString().Equals("F1"))
                    keysToSend += "{F1}";
                else if (e.KeyCode.ToString().Equals("F2"))
                    keysToSend += "{F2}";
                else if (e.KeyCode.ToString().Equals("F3"))
                    keysToSend += "{F3}";
                else if (e.KeyCode.ToString().Equals("F4"))
                    keysToSend += "{F4}";
                else if (e.KeyCode.ToString().Equals("F5"))
                    keysToSend += "{F5}";
                else if (e.KeyCode.ToString().Equals("F6"))
                    keysToSend += "{F6}";
                else if (e.KeyCode.ToString().Equals("F7"))
                    keysToSend += "{F7}";
                else if (e.KeyCode.ToString().Equals("F8"))
                    keysToSend += "{F8}";
                else if (e.KeyCode.ToString().Equals("F9"))
                    keysToSend += "{F9}";
                else if (e.KeyCode.ToString().Equals("F10"))
                    keysToSend += "{F10}";
                else if (e.KeyCode.ToString().Equals("F11"))
                    keysToSend += "{F11}";
                else if (e.KeyCode.ToString().Equals("F12"))
                    keysToSend += "{F12}";
                else if (e.KeyValue == 186)
                    keysToSend += "{;}";
                else if (e.KeyValue == 222)
                    keysToSend += "'";
                else if (e.KeyValue == 191)
                    keysToSend += "/";
                else if (e.KeyValue == 190)
                    keysToSend += ".";
                else if (e.KeyValue == 188)
                    keysToSend += ",";
                else if (e.KeyValue == 219)
                    keysToSend += "{[}";
                else if (e.KeyValue == 221)
                    keysToSend += "{]}";
                else if (e.KeyValue == 220)
                    keysToSend += "\\";
                else if (e.KeyValue == 187)
                    keysToSend += "{=}";
                else if (e.KeyValue == 189)
                    keysToSend += "{-}";
                //else
                    //keysToSend += e.KeyCode;
                e.SuppressKeyPress = true;
                eventSender.Write(keysToSend + "\n");
                eventSender.Flush();
            }
            catch (Exception)
            {
                //MessageBox.Show("An Error Occured While Sending a Key\n\n" + e.ToString(), "Failed to send key");
            }
        }

        private void Form2_MouseMove(object sender, MouseEventArgs e)
        {
        }

        //detects mouse movement and sends the coordinates to the server
        private void theImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (!sendKeysAndMouse)
                return;
            try
            {
                //get the mouse coordinates
                float correctX = (float) resolutionX * ((float) e.Location.X / theImage.Width);
                float correctY = (float) resolutionY * ((float) e.Location.Y / theImage.Height);
                correctX = ((int)correctX);
                correctY = ((int)correctY);
                //send the mouse coordinates
                eventSender.Write("M" + correctX + " " + correctY + "\n");
                eventSender.Flush();
            }
            catch (Exception)
            {
                //MessageBox.Show("An Error Occured While Sending a Mouse Coordinate\n\n" + gay.ToString(), "Oops. Mouse Failed to Send");
            }

        }

        private void theImage_MouseClick(object sender, MouseEventArgs e)
        {
            if (!sendKeysAndMouse)
                return;
            eventSender.Write("LCLICK\n");
            eventSender.Flush();
        }

        private void theImage_MouseDown(object sender, MouseEventArgs e)
        {
            if (!sendKeysAndMouse)
                return;
            eventSender.Write("LDOWN\n");
            eventSender.Flush();
        }

        private void theImage_MouseUp(object sender, MouseEventArgs e)
        {
            if (!sendKeysAndMouse)
                return;
            eventSender.Write("LUP\n");
            eventSender.Flush();
        }

        private void menuFileConnect_Click(object sender, EventArgs e)
        {
            //startConnect();
            connectDialog = new frmConnectDialog(this);
            connectDialog.ShowDialog(this);
            menuFileDisconnect.Enabled = true;
            menuFileConnect.Enabled = false;
        }

        private void controlMouseAndKeyboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sendKeysAndMouse = true;
        }

        private void shutdownServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (connected)
            {
                DialogResult reallyShutdown = MessageBox.Show("Shutting down the server will leave you unable to reconnect unless " +
                            "the the server is set to run on Startup. You will be unable " +
                            "to reconnect until this occurs.\n Continue?", "Warning!", MessageBoxButtons.YesNo);
                if (reallyShutdown == DialogResult.Yes)
                    sendShutdown();
                return;
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            sendKeysAndMouse = false;
        }

        private void menuFileDisconnect_Click(object sender, EventArgs e)
        {
            if (connected)
            {
                connector.Close();
                connected = false;

                statusLabel.Text = "Connection terminated";
                menuFileDisconnect.Enabled = false;
                menuFileConnect.Enabled = true;
            }
        }

        private void menuFileQuit_Click(object sender, EventArgs e)
        {
            //this.Dispose();
            if (connected)
            {
                connector.Close();
            }
            System.Environment.Exit(0);
        }

        private void showHideStatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (statusStrip1.Visible)
                statusStrip1.Visible = false;
            else
                statusStrip1.Visible = true;
        }

        private void menuHelpAbout_Click(object sender, EventArgs e)
        {
            aboutDialog = new AboutBox1();
            aboutDialog.ShowDialog(this);
        }

        private void updateIntervalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            updateDialog = new frmUpdateInterval();
            updateDialog.ShowDialog(this);
        }
    }
}
