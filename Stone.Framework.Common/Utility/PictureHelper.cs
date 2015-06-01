using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Web;
using System.Web.UI;

namespace Stone.Framework.Common.Utility
{
    public class PictureHelper
    {
        public static void GetThum(Page page, Size size, String sourceFile)
        {
            BuilderThumb(page, size, sourceFile, String.Empty, false, new Color());
        }

        private static void BuilderThumb(Page page, Size size, String sourceFile, String targetFile, Boolean Enalebgcolor, Color bgColor)
        {
            Int32 width = size.Width;
            Int32 height = size.Height;
            Image image = Image.FromFile(sourceFile);
            ImageFormat rawFormat = image.RawFormat;
            Size thumbSize = GetThumbSize(new Size(width, height), new Size(image.Width, image.Height));
            Bitmap bitmap = new Bitmap(thumbSize.Width, thumbSize.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            if (Enalebgcolor)
            {
                graphics.Clear(bgColor);
            }
            graphics.DrawImage(image, new Rectangle(0, 0, thumbSize.Width, thumbSize.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
            graphics.Dispose();
            EncoderParameters encoderParams = new EncoderParameters();
            Int64 num3 = 100;
            EncoderParameter parameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, num3);
            encoderParams.Param[0] = parameter;
            ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo encoder = null;
            for (Int32 i = 0; i < (imageEncoders.Length - 1); i++)
            {
                if (imageEncoders[i].FormatDescription.Equals("JPEG"))
                {
                    encoder = imageEncoders[i];
                    break;
                }
            }
            if (page != null)
            {
                HttpResponse response = page.Response;
                response.Clear();
                if (rawFormat.Equals(ImageFormat.Gif))
                {
                    response.ContentType = "image/gif";
                }
                else
                {
                    response.ContentType = "image/jpeg";
                }
                if (encoder != null)
                {
                    bitmap.Save(response.OutputStream, encoder, encoderParams);
                }
                else
                {
                    bitmap.Save(response.OutputStream, rawFormat);
                }
            }
            else
            {
                bitmap.Save(targetFile, rawFormat);
            }
        }

        public static void CreateThumb(Size size, String sourceFile, String targetFile)
        {
            BuilderThumb(null, size, sourceFile, targetFile, false, new Color());
        }

        private static Size GetThumbSize(Size max, Size thumb)
        {
            Double width = 0.0;
            Double height = 0.0;
            Double num3 = Convert.ToDouble(thumb.Width);
            Double num4 = Convert.ToDouble(thumb.Height);
            Double num5 = Convert.ToDouble(max.Width);
            Double num6 = Convert.ToDouble(max.Height);
            if ((num3 < num5) && (num4 < num6))
            {
                width = num3;
                height = num4;
            }
            else if ((num3 / num4) > (num5 / num6))
            {
                width = max.Width;
                height = (width * num4) / num3;
            }
            else
            {
                height = max.Height;
                width = (height * num3) / num4;
            }
            return new Size(Convert.ToInt32(width), Convert.ToInt32(height));
        }


        private enum OutputType
        {
            File,

            Page,
        }

        public static void cutImageToFile(string originalImagePath, string thumbnailPath, int width, int height)
        {
            cutImage(null, originalImagePath, thumbnailPath, width, height, OutputType.File);
        }

        public static void cutImageToPage(string url, int width, int height)
        {
            cutImage(url, null, null, width, height, OutputType.Page);
        }

        private static void cutImage(string url, string originalImagePath, string thumbnailPath, int width, int height, OutputType type)
        {
            //load image
            Image image = null;

            if (type == OutputType.Page)
            {
                WebRequest request = (WebRequest)HttpWebRequest.Create(url);
                WebResponse response = request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    image = Image.FromStream(stream);
                }
            }
            else
            {
                image = Image.FromFile(originalImagePath);
            }

            //need process?
            if (image.Width <= width && image.Height <= height)
            {
                if (type == OutputType.Page)
                {
                    HttpContext.Current.Response.ContentType = "image/jpeg";
                    image.Save(HttpContext.Current.Response.OutputStream, ImageFormat.Jpeg);
                }
                else
                {
                    image.Save(thumbnailPath, ImageFormat.Jpeg);
                }
                return;
            }

            //process
            int num = width;
            int num2 = height;
            int x = 0;
            int y = 0;
            int num5 = image.Width;
            int num6 = image.Height;
            if (num >= num2)
            {
                num2 = (num6 * num) / num5;
                if (num2 < height)
                {
                    num2 = height;
                    num = (num5 * num2) / num6;
                }
            }
            else
            {
                num = (num5 * num2) / num6;
                if (num < width)
                {
                    num = width;
                    num2 = (num6 * num) / num5;
                }
            }

            Image image2 = new Bitmap(num, num2);
            Graphics graphics = Graphics.FromImage(image2);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.Clear(Color.Transparent);
            graphics.DrawImage(image, new Rectangle(0, 0, num, num2), new Rectangle(x, y, num5, num6), GraphicsUnit.Pixel);
            num6 = height;
            num5 = width;
            y = (num2 - num6) / 2;
            x = (num - num5) / 2;

            Image image3 = new Bitmap(width, height);

            Graphics graphics2 = Graphics.FromImage(image3);

            graphics2.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics2.SmoothingMode = SmoothingMode.AntiAlias;
            graphics2.Clear(Color.Transparent);
            graphics2.DrawImage(image2, new Rectangle(0, 0, width, height), new Rectangle(x, y, num5, num6), GraphicsUnit.Pixel);
            image3 = KiSharpen((Bitmap)image3, float.Parse("0.5"));     //KiContrast(KiLighten(KiSharpen((Bitmap)image3, float.Parse("0.5")), 5), 10);

            int num7 = 100;

            ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo encoder = null;
            foreach (ImageCodecInfo info2 in imageEncoders)
            {
                if (info2.MimeType == "image/jpeg")
                {
                    encoder = info2;
                }
            }

            EncoderParameters encoderParams = new EncoderParameters();
            encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)num7);
            Bitmap img = new Bitmap(image3);

            try
            {
                if (type == OutputType.Page)
                {
                    MemoryStream ms = new MemoryStream();

                    img.Save(ms, ImageFormat.Jpeg);

                    HttpContext.Current.Response.ContentType = "image/jpeg";

                    ms.WriteTo(HttpContext.Current.Response.OutputStream);
                }
                else
                {
                    img.Save(thumbnailPath, ImageFormat.Jpeg);
                }

            }

            catch (Exception)
            {

            }
            finally
            {
                image.Dispose();
            }

        }

        /// <summary>
        /// 对比度
        /// </summary>
        /// <param name="b"></param>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static Bitmap KiContrast(Bitmap b, int degree)
        {
            if (b == null)
            {
                return null;
            }

            if (degree < -100)
            {
                degree = -100;
            }

            if (degree > 100)
            {
                degree = 100;
            }

            try
            {
                double num = 0.0;
                double num2 = (100.0 + degree) / 100.0;
                num2 *= num2;
                int width = b.Width;
                int height = b.Height;

                BitmapData bitmapdata = b.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                byte* numPtr = (byte*)bitmapdata.Scan0;

                int num5 = bitmapdata.Stride - (width * 3);
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            num = ((((((double)numPtr[k]) / 255.0) - 0.5) * num2) + 0.5) * 255.0;

                            if (num < 0.0)
                            {
                                num = 0.0;
                            }

                            if (num > 255.0)
                            {
                                num = 255.0;
                            }

                            numPtr[k] = (byte)num;
                        }

                        numPtr += 3;
                    }

                    numPtr += num5;
                }

                b.UnlockBits(bitmapdata);
                return b;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 亮度
        /// </summary>
        /// <param name="b"></param>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static Bitmap KiLighten(Bitmap b, int degree)
        {
            if (b == null)
            {
                return null;
            }

            if (degree < -255)
            {
                degree = -255;
            }

            if (degree > 0xff)
            {
                degree = 0xff;
            }

            try
            {
                int width = b.Width;
                int height = b.Height;
                int num3 = 0;

                BitmapData bitmapdata = b.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                byte* numPtr = (byte*)bitmapdata.Scan0;
                int num4 = bitmapdata.Stride - (width * 3);
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            num3 = numPtr[k] + degree;

                            if (degree < 0)
                            {
                                numPtr[k] = (byte)Math.Max(0, num3);
                            }

                            if (degree > 0)
                            {
                                numPtr[k] = (byte)Math.Min(0xff, num3);
                            }
                        }
                        numPtr += 3;
                    }
                    numPtr += num4;
                }
                b.UnlockBits(bitmapdata);
                return b;
            }
            catch
            {
                return null;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static Bitmap KiSharpen(Bitmap b, float val)
        {
            if (b == null)
            {
                return null;
            }

            int width = b.Width;
            int height = b.Height;
            try
            {
                Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);

                BitmapData bitmapdata = b.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                BitmapData data2 = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

                byte* numPtr = (byte*)bitmapdata.Scan0.ToPointer();

                byte* numPtr2 = (byte*)data2.Scan0.ToPointer();

                int stride = bitmapdata.Stride;

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if ((((j == 0) || (j == (width - 1))) || (i == 0)) || (i == (height - 1)))
                        {
                            numPtr2[0] = numPtr[0];
                            numPtr2[1] = numPtr[1];
                            numPtr2[2] = numPtr[2];
                        }

                        else
                        {
                            byte* numPtr3 = (numPtr - stride) - 3;
                            int num6 = numPtr3[2];
                            int num15 = numPtr3[1];
                            int num24 = numPtr3[0];
                            numPtr3 = numPtr - stride;
                            int num7 = numPtr3[2];
                            int num16 = numPtr3[1];
                            int num25 = numPtr3[0];
                            numPtr3 = (numPtr - stride) + 3;
                            int num8 = numPtr3[2];
                            int num17 = numPtr3[1];
                            int num26 = numPtr3[0];
                            numPtr3 = numPtr - 3;

                            int num9 = numPtr3[2];
                            int num18 = numPtr3[1];
                            int num27 = numPtr3[0];
                            numPtr3 = numPtr + 3;
                            int num10 = numPtr3[2];
                            int num19 = numPtr3[1];
                            int num28 = numPtr3[0];
                            numPtr3 = (numPtr + stride) - 3;
                            int num11 = numPtr3[2];
                            int num20 = numPtr3[1];
                            int num29 = numPtr3[0];
                            numPtr3 = numPtr + stride;
                            int num12 = numPtr3[2];
                            int num21 = numPtr3[1];
                            int num30 = numPtr3[0];
                            numPtr3 = (numPtr + stride) + 3;

                            int num13 = numPtr3[2];
                            int num22 = numPtr3[1];
                            int num31 = numPtr3[0];

                            numPtr3 = numPtr;

                            int num14 = numPtr3[2];
                            int num23 = numPtr3[1];
                            int num32 = numPtr3[0];
                            float num33 = num14 - (((float)(((((((num6 + num7) + num8) + num9) + num10) + num11) + num12) + num13)) / 8f);
                            float num34 = num23 - (((float)(((((((num15 + num16) + num17) + num18) + num19) + num20) + num21) + num22)) / 8f);
                            float num35 = num32 - (((float)(((((((num24 + num25) + num26) + num27) + num28) + num29) + num30) + num31)) / 8f);

                            num33 = num14 + (num33 * val);
                            num34 = num23 + (num34 * val);
                            num35 = num32 + (num35 * val);

                            if (num33 > 0f)
                            {
                                num33 = Math.Min(255f, num33);
                            }
                            else
                            {
                                num33 = Math.Max(0f, num33);
                            }

                            if (num34 > 0f)
                            {
                                num34 = Math.Min(255f, num34);
                            }
                            else
                            {
                                num34 = Math.Max(0f, num34);
                            }

                            if (num35 > 0f)
                            {
                                num35 = Math.Min(255f, num35);
                            }

                            else
                            {
                                num35 = Math.Max(0f, num35);
                            }

                            numPtr2[0] = (byte)num35;
                            numPtr2[1] = (byte)num34;
                            numPtr2[2] = (byte)num33;
                        }

                        numPtr += 3;
                        numPtr2 += 3;
                    }

                    numPtr += bitmapdata.Stride - (width * 3);
                    numPtr2 += bitmapdata.Stride - (width * 3);
                }
                b.UnlockBits(bitmapdata);

                bitmap.UnlockBits(data2);

                return bitmap;
            }
            catch
            {
                return null;
            }
        }
    }
}
