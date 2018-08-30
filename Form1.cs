using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;

namespace TestForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            m_fnReceiveHandler = new AsyncCallback(handleDataReceive);
            m_fnSendHandler = new AsyncCallback(handleDataSend);
        }

        public class AsyncObject
        {
            public Byte[] Buffer;
            public Socket WorkingSocket;
            public AsyncObject(Int32 bufferSize)
            {
                this.Buffer = new byte[bufferSize];
            }


        }



        private Socket m_ClientSocket = null;
        private Boolean g_Connected;
        private AsyncCallback m_fnReceiveHandler;
        private AsyncCallback m_fnSendHandler;


        private void handleDataReceive(IAsyncResult ar)
        {

        }

        private void handleDataSend(IAsyncResult ar)
        {

        }

        public Boolean Connected
        {
            get
            {
                return g_Connected;
            }
        }

        public void ConnectToServer(String hostName, UInt16 hostPort)
        {
            m_ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            Boolean isConnected = false;
            try
            {
                m_ClientSocket.Connect(hostName, hostPort);

                isConnected = true;
            }
            catch
            {
                isConnected = false;
            }
            g_Connected = isConnected;

            if (isConnected)
            {
                AsyncObject ao = new AsyncObject(4096);

                ao.WorkingSocket = m_ClientSocket;

                m_ClientSocket.BeginReceive(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_fnReceiveHandler, ao);

                Console.WriteLine("연결 성공!");
            }
            else
            {
                Console.WriteLine("연결 실패!");
            }
        }

        public void StopClient()
        {
            m_ClientSocket.Close();
        }

        public void SendMessage(String message)
        {
            AsyncObject ao = new AsyncObject(1);

            ao.Buffer = Encoding.Unicode.GetBytes(message);

            ao.WorkingSocket = m_ClientSocket;

            try
            {
                m_ClientSocket.BeginSend(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_fnSendHandler, ao);
            }catch(Exception ex)
            {
                Console.WriteLine("전송 중 오류발생");
            }
        }

        private void handleDataReceive(IAsyncResult ar)
        {
            AsyncObject ao = (AsyncObject)ar.AsyncState;

            Int32 recvBytes;

            try
            {
                recvBytes = ao.WorkingSocket.EndReceive(ar);
            }
            catch
            {
                return;
            }

            if(recvBytes>0)
        }
        


        FilterInfoCollection _videoDevices;
        

        private void Form1_Load(object sender, EventArgs e)
        {
            _videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if(_videoDevices.Count == 0)
            {
                button1.Enabled = false;
                MessageBox.Show("No video input device");
                return;
            }
        }




        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = eventArgs.Frame;

            this.Invoke((Action)(() =>
            {
                pictureBox1.Image = (Bitmap) bitmap.Clone();
            }), null);
        }

        VideoCaptureDevice _videoSource;
        VideoCapabilities[] videoCapabilities;

        private void button1_Click(object sender, EventArgs e)
        {
            if(button1.Text == "Start")
            {
                //MessageBox.Show(_videoDevices[0].ToString());
                //Console.WriteLine(_videoDevices[0].ToString());
                _videoSource = new VideoCaptureDevice(_videoDevices[0].MonikerString);
                _videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
                videoCapabilities = _videoSource.VideoCapabilities;

                //int i = 1;

                //foreach(VideoCapabilities cap in videoCapabilities)
                //{
                //   MessageBox.Show(i + "번째  " + cap.FrameSize.Width + ", " + cap.FrameSize.Height);
                //    i++;
                //}
                

                _videoSource.VideoResolution = videoCapabilities[19];
              
                _videoSource.Start();

                button1.Text = "Stop";
            }
            else
            {
                _videoSource.SignalToStop();
                button1.Text = "Start";
            }
        }
    }
}
