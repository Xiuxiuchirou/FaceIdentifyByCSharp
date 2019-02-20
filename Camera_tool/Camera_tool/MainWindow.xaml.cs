using Camera_driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Camera_tool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Camera camera = new Camera();
        DispatcherTimer showImageTimer;
        DispatcherTimer identifyFaceTimer;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void open_Click(object sender, RoutedEventArgs e)
        {
            
            camera.openCamera();
            //if (showImageTimer == null)
            //{
            //    showImageTimer = new DispatcherTimer();
            //    showImageTimer.Interval = TimeSpan.FromMilliseconds(2);
            //    showImageTimer.Tick += TakingVideo;
            //    showImageTimer.Start();
            //}
            //else if (showImageTimer.IsEnabled == false)
            //{
            //    showImageTimer.Start();
            //}

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

        private void close_Click(object sender, RoutedEventArgs e)
        {
            if(showImageTimer != null && showImageTimer.IsEnabled == true)
            {
                showImageTimer.Stop();
            }
            if(identifyFaceTimer != null && identifyFaceTimer.IsEnabled == true)
            {
                identifyFaceTimer.Stop();
            }
            
            camera.closeCamera();
        }

        private void TakingVideo(object sender, EventArgs e)
        {
            picShow.Source = camera.getImage();

        }


        private void identifyFace(object sender, EventArgs e)
        {
            CameraInfo cameraInfo = camera.identifyFace();
            if(cameraInfo != null)
            {
                if (cameraInfo.isHasFace)
                {
                    Console.WriteLine("---------检测到人脸------");
                    picShow.Source = cameraInfo.imagesSource;
                    // TODO something
                }
                else
                {
                    Console.WriteLine("未检测到人脸");
                    picShow.Source = cameraInfo.imagesSource;
                }
            }
            
        }

    }
}
