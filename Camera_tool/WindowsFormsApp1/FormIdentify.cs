using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Camera_driver; 
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;

namespace WindowsFormsApp1
{
    public partial class FormIdentify : Form
    {
        public FormIdentify()
        {
            InitializeComponent();
        }
        Camera camera = new Camera();
        DispatcherTimer showImageTimer;
        DispatcherTimer identifyFaceTimer;

        /// <summary>
        /// 开始识别
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            camera.openCamera(); 

            // 人脸识别定时器
            if (identifyFaceTimer == null)
            {
                identifyFaceTimer = new DispatcherTimer();
                identifyFaceTimer.Interval = TimeSpan.FromMilliseconds(20);
                identifyFaceTimer.Tick += identifyFace;
                identifyFaceTimer.Start();
            }
            else if (identifyFaceTimer.IsEnabled == false)
            {
                identifyFaceTimer.Start();
            }
        }

        private void identifyFace(object sender, EventArgs e)
        {
            CameraInfo cameraInfo = camera.identifyFace();
            if (cameraInfo != null)
            {
                if (cameraInfo.isHasFace)
                {
                    Console.WriteLine("---------检测到人脸------");
                    picShow.Image = ImageSourceToBitmap(cameraInfo.imagesSource);
                    // TODO something
                }
                else
                {
                    Console.WriteLine("未检测到人脸");
                    picShow.Image = ImageSourceToBitmap(cameraInfo.imagesSource);
                }
            }

        }

        public static System.Drawing.Bitmap ImageSourceToBitmap(ImageSource imageSource)
        {
            BitmapSource m = (BitmapSource)imageSource;

            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(m.PixelWidth, m.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb); // 坑点：选Format32bppRgb将不带透明度

            System.Drawing.Imaging.BitmapData data = bmp.LockBits(
            new System.Drawing.Rectangle(System.Drawing.Point.Empty, bmp.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            m.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bmp.UnlockBits(data);

            return bmp;
        }

    }
}
