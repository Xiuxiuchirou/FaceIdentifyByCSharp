using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows;
using System.Windows.Media;

namespace Camera_driver
{
    public class Camera
    {
        private static CascadeClassifier faceClassifier = new CascadeClassifier(AppDomain.CurrentDomain.BaseDirectory + "haarcascade_frontalface_default.xml");

        Capture capture;    //摄像头对象


        public void openCamera()
        {

            if(capture == null)
            {
                capture = new Capture(0);
            }
        }

        public ImageSource getImage()
        {
            try
            {
                Mat mat = capture.QueryFrame();
                return ImageConverterL.ToBitmapSource(mat.Bitmap);
            }
            catch(Exception)
            {
                return null;
            }
        }

        public void closeCamera()
        {
            capture.Dispose();
            capture = null;
        }

        public CameraInfo identifyFace()
        {
            CameraInfo info = new CameraInfo();
            using(Mat mat = capture.QueryFrame())
            {
                if(mat == null)
                {
                    return null;
                }
                List<Rectangle> faces = getFaceRectangle(mat);
                if (faces.Count <= 0)
                {
                    info.isHasFace = false;
                }
                else
                {
                    // 绘制人脸
                    using (Graphics g = Graphics.FromImage(mat.Bitmap))
                    {
                        foreach (Rectangle face in faces)
                        {
                            g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Red, 2), face);//给识别出的人脸画矩形框
                        }
                    }
                    info.isHasFace = true;             
                }
                info.mat = mat;
                info.imagesSource = ImageConverterL.ToBitmapSource(mat.Bitmap);

                return info;
            } 
        }

        /// <summary>
        /// 获取人脸矩形
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        private List<Rectangle> getFaceRectangle(Mat mat)
        {
            List<Rectangle> faces = new List<Rectangle>();
            try
            {
                using (UMat ugray = new UMat())
                {
                    CvInvoke.CvtColor(mat, ugray, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);//灰度化图片
                    CvInvoke.EqualizeHist(ugray, ugray);//均衡化灰度图片

                    Rectangle[] facesDetected = faceClassifier.DetectMultiScale(ugray, 1.1, 10, new System.Drawing.Size(20, 20));
                    faces.AddRange(facesDetected);
                }
            }
            catch (Exception)
            {
            }
            return faces;
        }

        


        class ImageConverterL
        {
            /// <summary>
            /// Delete a GDI object
            /// </summary>
            /// <param name="o">The poniter to the GDI object to be deleted</param>
            /// <returns></returns>
            [DllImport("gdi32")]
            private static extern int DeleteObject(IntPtr o);

            public static BitmapSource ToBitmapSource(Bitmap image)
            {
                IntPtr ptr = image.GetHbitmap();
                BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ptr, IntPtr.Zero,
                                                                                               Int32Rect.Empty,
                                                                                               System.Windows.Media.Imaging.
                                                                                                      BitmapSizeOptions.
                                                                                                      FromEmptyOptions());
                DeleteObject(ptr);
                return bs;
            }
        }

    }
}
