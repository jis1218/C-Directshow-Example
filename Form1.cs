using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Threading;

namespace TestForms
{
    delegate void MyDelegate(string s);

    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();

        }


        FilterInfoCollection _videoDevices;
        AWSImageRekognition imageRekognition;
        MyDelegate del;

        private void Form1_Load(object sender, EventArgs e)
        {
            _videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            imageRekognition = new AWSImageRekognition();
            imageRekognition.initialize();
            del = delegate (string s)
            {                
                this.Invoke((Action)(() =>
                {
                    label5.Text = s;
                }), null);
            };

            if(_videoDevices.Count == 0)
            {
                button1.Enabled = false;
                MessageBox.Show("No video input device");
                return;
            }

            _videoSource = new VideoCaptureDevice(_videoDevices[0].MonikerString);
            _videoSource.SetCameraProperty(CameraControlProperty.Zoom, 100, CameraControlFlags.Auto);
            _videoSource.SetCameraProperty(CameraControlProperty.Exposure, -7, CameraControlFlags.Auto);
            _videoSource.SetCameraProperty(CameraControlProperty.Focus, 0, CameraControlFlags.Manual);
            videoCapabilities = _videoSource.VideoCapabilities;


            foreach (VideoCapabilities cap in videoCapabilities)
            {
                comboBox1.Items.Add(string.Format("{0} x {1}", cap.FrameSize.Width, cap.FrameSize.Height));
                
            }

            comboBox1.SelectedIndex = 0;
        
        }


        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = eventArgs.Frame;

                //this.Invoke((Action)(() =>
                //{
                //    if (pictureBox1.Image != null)
                //        pictureBox1.Image.Dispose();

                //    pictureBox1.Image = (Bitmap) bitmap.Clone();
                //}), null);

            Thread t1 = new Thread(new ThreadStart(Run));
            t1.Start(bitmap);
        }


        public void changeLabel(String s)
        {
            label1.Text = s;
        }

        VideoCaptureDevice _videoSource;
        VideoCapabilities[] videoCapabilities;

        private void button1_Click(object sender, EventArgs e)
        {
            if(button1.Text == "Start")
            {
                //MessageBox.Show(_videoDevices[0].ToString());
                //Console.WriteLine(_videoDevices[0].ToString());
                _videoSource.VideoResolution = videoCapabilities[comboBox1.SelectedIndex];
                //_videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
                
                
                
                _videoSource.GetCameraProperty(CameraControlProperty.Zoom, out int zoom, out CameraControlFlags controlFlags);
                _videoSource.GetCameraProperty(CameraControlProperty.Exposure, out int exposure , out CameraControlFlags controlFlags2);
                _videoSource.GetCameraProperty(CameraControlProperty.Focus, out int focus, out CameraControlFlags controlFlags3);
                _videoSource.GetCameraProperty(CameraControlProperty.Iris, out int iris , out CameraControlFlags controlFlags4);
                _videoSource.GetCameraProperty(CameraControlProperty.Roll, out int roll, out CameraControlFlags controlFlags5);

                //_videoSource.Start();
                modifiedVideoSourcePlayer1.VideoSource = _videoSource;
                modifiedVideoSourcePlayer1.Start();
                sendImageToAWS();

                button1.Text = "Stop";
            }
            else
            {
                _videoSource.SignalToStop();
                sendImageToAWS();
                button1.Text = "Start";
            }
        }

        private void sendImageToAWS()
        {

            Task.Factory.StartNew(new Action(Run));
            

            //Thread t1 = null;
            //if (t1 == null)
            //{
            //    t1 = new Thread(new ThreadStart(Run));
            //    t1.Start();
            //}
            //else
            //{
            //    t1.Abort();
            //}
            
            
        }

        private void Run()
        {
            while (true)
            {
                Bitmap bitmap = modifiedVideoSourcePlayer1.GetCurrentVideoFrame();
                imageRekognition.requestRekognition((Bitmap)bitmap, del);
                Thread.Sleep(1000);
            }
            
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            System.Windows.Forms.TrackBar myTb;
            myTb = (System.Windows.Forms.TrackBar)sender;

            _videoSource.SetCameraProperty(CameraControlProperty.Zoom, myTb.Value, CameraControlFlags.Manual);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            System.Windows.Forms.TrackBar myTb;
            myTb = (System.Windows.Forms.TrackBar)sender;

            _videoSource.SetCameraProperty(CameraControlProperty.Exposure, myTb.Value, CameraControlFlags.Manual);
        }



        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            System.Windows.Forms.TrackBar myTb;
            myTb = (System.Windows.Forms.TrackBar)sender;

            _videoSource.SetCameraProperty(CameraControlProperty.Focus, myTb.Value, CameraControlFlags.Manual);
        }
    }
}
