using System;
using System.Collections.Generic;
using System.Linq;
using Alturos.Yolo;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV.UI;
using Alturos.Yolo.Model;

namespace ConsoleApp1
{
    class Program
    {
        public static void Main(string[] args)
        {
            Capturer call1 = new Capturer();
            call1.CaptureIt();

        }


        class Capturer

        {
            public void CaptureIt()
            {

                ImageViewer viewer = new ImageViewer();
                VideoCapture capture = new VideoCapture();
                var font = new Font("Courier", 18, FontStyle.Italic);
                var brush = new SolidBrush(Color.White);
                var pen = new Pen(Color.Blue, 6);
                void addDetails(Bitmap image, List<YoloItem> yolotem)
                {
                    var graphic = Graphics.FromImage(image);
                    foreach (var item in yolotem)
                    {
                        var width = item.Width;
                        var height = item.Height;
                        var type = item.Type;
                        var confidence = item.Confidence;
                        var box = new Rectangle(item.X, item.Y, width, height);
                        var point = new Point(item.X, item.Y);
                        var str = type + "  " + confidence;
                        graphic.DrawRectangle(pen, box);
                        graphic.DrawString(str, font, brush, point);
                    }

                }
                using (var yoloWrapper = new YoloWrapper("yolov3-tiny.cfg", "yolov3-tiny.weights", "coco.names"))
                {
                    Application.Idle += new EventHandler(delegate (object sender, EventArgs e)
                    {
                        var nextFrame = capture.QueryFrame();    
                        using (Bitmap bimp = nextFrame.Bitmap)
                        {
                            var memory = new System.IO.MemoryStream();
                            bimp.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                            var items = yoloWrapper.Detect(memory.ToArray()).ToList();
                            addDetails(bimp, items);
                            Image<Bgr, Byte> detailed = new Image<Bgr, Byte>(bimp);
                            viewer.Image = detailed;
                            detailed.Dispose();
                            viewer.Image.Dispose();
                        }

                    });
                    viewer.ShowDialog();
                }
            }
        }


    }
}
