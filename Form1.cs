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

namespace TestForms
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
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
