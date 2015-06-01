
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Web;

namespace Stone.Framework.Common.Utility
{
    /// <summary>
    /// 验证码
    /// </summary>
    public class AuthCode
    {
        private const int Width = 180, Height = 55; //宽度和高度

        private static readonly string[] FontFamily =
        {
            "Arial", "Arial Black", "Arial Italic", "Courier New",
            "Courier New Bold Italic", "Courier New Italic",
            "Franklin Gothic Medium", "Franklin Gothic Medium Italic"
        };

        private static readonly int[] FontSize = { 20, 25, 30 };

        private static readonly Color[] ColorFace =
        {
            Color.FromArgb(113, 153, 67), Color.FromArgb(30, 99, 140),
            Color.FromArgb(206, 60, 19), Color.FromArgb(227, 60, 0)
        };

        private static readonly Color[] ColorBack =
        {
            Color.FromArgb(247, 254, 236), Color.FromArgb(234, 248, 255),
            Color.FromArgb(244, 250, 246), Color.FromArgb(248, 248, 248)
        };

        private static readonly StringFormat TextFormat = new StringFormat(StringFormatFlags.NoClip); //文本布局信息
        private static readonly int _angle = 60; //左右旋转角度

        public static void CreateImage(string code, HttpContext context)
        {
            TextFormat.Alignment = StringAlignment.Center;
            TextFormat.LineAlignment = StringAlignment.Center;

            var tick = DateTime.Now.Ticks;
            var rnd = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
            using (var img = new Bitmap(Width, Height))
            {
                using (var g = Graphics.FromImage(img))
                {
                    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    var dot = new Point(20, 20);
                    var nor = rnd.Next(53);
                    var rsta = rnd.Next(130);
                    var m = rnd.Next(15) + 5;
                    var d = rnd.Next(20) + 15;
                    var v = rnd.Next(5) + 1; // 绘制干扰正弦曲线 M:曲线平折度, D:Y轴常量 V:X轴焦距
                    var colorIndex = rnd.Next(4);
                    var pxX = 0.0F;
                    var pxY = Convert.ToSingle(m * Math.Sin(v * pxX * Math.PI / 180) + d);
                    float pyX, pyY;
                    g.Clear(ColorBack[rnd.Next(4)]);

                    using (Brush brushFace = new SolidBrush(ColorFace[colorIndex]))
                    {
                        #region 绘制正弦线
                        for (int i = 0; i < 131; i++)
                        {
                            pyX = pxX + 1;
                            pyY = Convert.ToSingle(m * Math.Sin(v * pyX * Math.PI / 180) + d);

                            if (rsta < i && i < (rsta + nor)) continue;
                            using (Pen pen = new Pen(brushFace, rnd.Next(2, 4) + 1.5F))
                            {
                                g.DrawLine(pen, pxX, pxY, pyX, pyY);
                            }
                            pxX = pyX;
                            pxY = pyY;
                        }
                        #endregion

                        g.TranslateTransform(18, 4);

                        foreach (char item in code)
                        {
                            int angle = rnd.Next(-_angle, _angle);
                            g.TranslateTransform(dot.X, dot.Y);
                            g.RotateTransform(angle);
                            using (Font font = new Font(FontFamily[rnd.Next(0, 8)], FontSize[rnd.Next(0, 3)]))
                            {
                                //绘制
                                g.DrawString(item.ToString(), font, brushFace, 1, 1, TextFormat);
                            }
                            g.RotateTransform(-angle);
                            g.TranslateTransform(-2, -dot.Y);
                        }
                    }
                }

                using (var ms = new MemoryStream())
                {
                    context.Response.ContentType = "Image/PNG";
                    context.Response.Clear();
                    context.Response.BufferOutput = true;
                    img.Save(ms, ImageFormat.Png);
                    ms.Flush();
                    context.Response.BinaryWrite(ms.GetBuffer());
                    context.Response.End();
                }
            }
        }
    }
}