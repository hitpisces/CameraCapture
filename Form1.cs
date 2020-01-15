using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using System.Timers;
using System.IO;

namespace CameraCapture
{
    public partial class Form1 : Form
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoDevice;
        private VideoCapabilities[] videoCapabilities;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            timer1.Stop();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count !=0)
            {
                foreach (FilterInfo device in videoDevices)
                {
                    comboBox1.Items.Add(device.Name);
                }
            }
            else
            {
                comboBox1.Items.Add("未发现摄像头！");
            }
            comboBox1.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (videoDevices.Count !=0)
            {
                videoDevice = new VideoCaptureDevice(videoDevices[comboBox1.SelectedIndex].MonikerString);
                GetDeviceResolution(videoDevice);
            }
        }

        private void GetDeviceResolution(VideoCaptureDevice videoCaptureDevice)
        {
            comboBox2.Items.Clear();
            videoCapabilities = videoCaptureDevice.VideoCapabilities;
            foreach (VideoCapabilities capabilty in videoCapabilities)
            {
                comboBox2.Items.Add($"{capabilty.FrameSize.Width} x {capabilty.FrameSize.Height}");
            }
            comboBox2.SelectedIndex = 0;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(textBox2.Text)>0 && Convert.ToInt32(textBox2.Text)<86400)
            {
                MessageBox.Show("确认时间参数？");
                textBox2.ReadOnly = true;
            }
            else
            {
                MessageBox.Show("请在时间参数中输入介于0~‭86400的正整数‬！");
            }

            if (Directory.Exists(textBox1.Text) is true)
            {
                MessageBox.Show("确认截图保存路径？");
                textBox1.ReadOnly = true;
            }
            else
            {
                MessageBox.Show("存储路径不存在！请确认存储路径！");
            }

            timer1.Enabled = true;
            timer1.Interval = Convert.ToInt32(textBox2.Text) * 1000;
     
        }

        private void EnableControlStatus(bool status)
        {
            comboBox1.Enabled = status;
            comboBox2.Enabled = status;
            button2.Enabled = status;
            ///btnShoot.Enabled = !status;
            ///btnDisconnect.Enabled = !status;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (videoDevice != null)
            {
                if ((videoCapabilities != null) && (videoCapabilities.Length != 0))
                {
                    videoDevice.VideoResolution = videoCapabilities[comboBox2.SelectedIndex];

                    videoSourcePlayer1.VideoSource = videoDevice;
                    videoSourcePlayer1.Start();
                    EnableControlStatus(false);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (videoSourcePlayer1.VideoSource != null)
            {
                videoSourcePlayer1.SignalToStop();
                videoSourcePlayer1.WaitForStop();
                videoSourcePlayer1.VideoSource = null;
            }
            this.Dispose();
            System.Environment.Exit(0);

        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8 )
            {
                 e.Handled = true;
            }
            else
            {
                e.Handled = false; 
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Bitmap img = videoSourcePlayer1.GetCurrentVideoFrame();
            pictureBox1.Image = img;
            string saveString = textBox1.Text.ToString() + DateTime.Now.ToString("yy-MM-dd-HH-mm-ss")+".png";
            img.Save(saveString,System.Drawing.Imaging.ImageFormat.Png);
        }
    }
    
}
