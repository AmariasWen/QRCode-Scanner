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
using ZXing;

namespace Scanner
{
    public partial class Form1 : Form
    {
        FilterInfoCollection videoDevices;
        VideoCaptureDevice videoSource;
        int selectedDeviceId = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void InitVideo()
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if(videoDevices.Count==0)//未检测到摄像头
            {
                return;
            }

            videoSource = new VideoCaptureDevice(videoDevices[selectedDeviceId].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(videoSource_NewFrame);
            videoSource.Start();
        }

 
        private void videoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            InitVideo();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            videoSource.SignalToStop();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
           if(videoSource.IsRunning)
               videoSource.SignalToStop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                timer1.Enabled = false;
                Bitmap img = (Bitmap)pictureBox1.Image.Clone();
                if (Decode(img))
                {
                    videoSource.SignalToStop();
                    timer1.Stop();
                }
                else
                    timer1.Enabled = true;
            }
        }

        private bool Decode(Bitmap b)
        {
            try
            {
                BarcodeReader reader = new BarcodeReader();
                reader.AutoRotate = true;
                Result result = reader.Decode(b);
                textBox1.Text = result.Text;
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
           timer1.Start();
        }
    }
}
