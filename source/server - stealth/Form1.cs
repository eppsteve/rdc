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
using System.Runtime.InteropServices;
using System.Threading;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Server
{
    public partial class Form1 : Form
    {
        // DllImport Provides Control Over The Mouse
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(long dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);

        // Constant Offsets for Mouse Methods
        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms646260(v=vs.85).aspx
        private const int MOUSE_LEFTDOWN = 0x02;
        private const int MOUSE_LEFTUP = 0x04;
        private const int MOUSE_RIGHTDOWN = 0x08;
        private const int MOUSE_RIGHTUP = 0x10;

        // Variable Declaration
        private TcpListener listener;       //Listening Port for the Server
        private Socket mainSocket;          //Socket used for connection
        private int port;                   //Port to listen on
        private Stream s;                   //Main Stream for Communication
        private Thread eventWatcher;        //Thread to Monitor Client Commands
        private int imageDelay;             //Delay in Milliseconds for Desktop Image Send

        public Form1()
        {
            InitializeComponent();
            port = 1818;
            imageDelay = 1000;
        }


        /// <summary>
        /// Starts listening for connections
        /// </summary>
        public void startListening() {
            while (true)
            {
                try
                {
                    // Update UI
                    this.Invoke((MethodInvoker)delegate {
                        txtLog.AppendText("Waiting for connections...\n");
                    });

                    listener = new TcpListener(port);
                    listener.Start();
                    mainSocket = listener.AcceptSocket();
                    s = new NetworkStream(mainSocket);

                    // Update UI
                    this.Invoke((MethodInvoker)delegate {
                        txtLog.AppendText("A new client has been connected!\n");
                    });

                    // Waits for commands in a new thread
                    eventWatcher = new Thread(new ThreadStart(waitForKeys));
                    eventWatcher.Start();

                    // Captures the screen and sends it to client
                    while (true)
                    {
                        Bitmap screeny = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb);
                        Graphics theShot = Graphics.FromImage(screeny);
                        theShot.ScaleTransform(.25F, .25F);
                        theShot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
                        BinaryFormatter bFormat = new BinaryFormatter();
                        bFormat.Serialize(s, screeny);
                        //screeny.Save(s,ImageFormat.Png);
                        Thread.Sleep(imageDelay);
                        theShot.Dispose();
                        screeny.Dispose();
                    }
                }
                catch (Exception)
                {
                    if (mainSocket.IsBound)
                        mainSocket.Close();
                    if (listener != null)
                        listener.Stop();
                    //MessageBox.Show(e.ToString());
                }
            }
        }
      
        /// <summary>
        /// Waits for commands from client
        /// </summary>
        private void waitForKeys() 
        {
            try
            {
                String temp = "";
                StreamReader reader = new StreamReader(s);
                do
                { 
                    temp = reader.ReadLine();
                    if (temp.StartsWith("CDELAY"))
                    {
                        imageDelay = int.Parse(temp.Substring(6  , temp.Length - 6));
                    }
                    else if (temp.StartsWith("LCLICK"))
                    {
                        mouse_event(MOUSE_LEFTDOWN | MOUSE_LEFTUP, Cursor.Position.X, Cursor.Position.Y, 0, 0);
                    }
                    else if (temp.StartsWith("RCLICK"))
                    {
                        mouse_event(MOUSE_RIGHTDOWN | MOUSE_RIGHTUP, Cursor.Position.X, Cursor.Position.Y, 0, 0);
                    }
                    else if (temp.StartsWith("DBLCLICK"))
                    {
                        mouse_event(MOUSE_LEFTDOWN | MOUSE_LEFTUP, Cursor.Position.X, Cursor.Position.Y, 0, 0);
                        Thread.Sleep(150);
                        mouse_event(MOUSE_LEFTDOWN | MOUSE_LEFTUP, Cursor.Position.X, Cursor.Position.Y, 0, 0);
                    }

                    else if (temp.StartsWith("SHUTDOWN"))
                    {
                        mainSocket.Close();
                        listener.Stop();
                        Environment.Exit(0);
                    }
                    else if (temp.StartsWith("LDOWN"))
                    {
                        mouse_event(MOUSE_LEFTDOWN, Cursor.Position.X, Cursor.Position.Y, 0, 0);
                    }
                    else if (temp.StartsWith("LUP"))
                    {
                        mouse_event(MOUSE_LEFTUP, Cursor.Position.X, Cursor.Position.Y, 0, 0);
                    }
                    else if (temp.StartsWith("M"))
                    {
                        int xPos = 0, yPos = 0;
                        try
                        {
                            xPos = int.Parse(temp.Substring(1, temp.IndexOf(' ')));
                            yPos = int.Parse(temp.Substring(temp.IndexOf(' '), temp.Length - temp.IndexOf(' ')));
                            Cursor.Position = new Point(xPos, yPos);
                            continue;
                        }
                        catch (Exception)
                        {
                            //MessageBox.Show(temp + " " + xPos + " " + yPos + "\n\n" + e.ToString());
                        }
                    }
                    //if (temp.StartsWith("m")) { MessageBox.Show(temp + " received!"); }
                    //MessageBox.Show(temp + " received!");
                    else
                    {
                        SendKeys.SendWait(temp);
                    }
                }
                while (temp != null);
            }
            catch (Exception)
            {
                //MessageBox.Show(e.ToString());
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Thread listenForConnections = new Thread(new ThreadStart(startListening));
            listenForConnections.Start();
        }
    }
}