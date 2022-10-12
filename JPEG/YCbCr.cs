using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JPEG
{
    public partial class YCbCr : Form
    {
        public float[,] Y;
        public float[,] Cb;
        public float[,] Cr;
        public float[,] newY;
        public float[,] newCb;
        public float[,] newCr;
        public int N;

        public YCbCr(float[,] Y1, float[,] Cb1, float[,] Cr1, float[,] Y2, float[,] Cb2, float[,] Cr2, int n)
        {
            Y = Y1;
            Cb = Cb1;
            Cr = Cr1;
            newY = Y2;
            newCb = Cb2;
            newCr = Cr2;
            N = n;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Исходное изображение            
            Color color;
            Graphics gY = pictureBox1.CreateGraphics();
            SolidBrush brushY = new SolidBrush(Color.Black);
            gY.Clear(BackColor);
            for (int i = 0; i < pictureBox1.Height; ++i)
            {
                for (int j = 0; j < pictureBox1.Width; ++j)
                {
                    int R = (int)Y[i, j];
                    int G = (int)Y[i, j];
                    int B = (int)Y[i, j];

                    if (R > 255) R = 255;
                    if (G > 255) G = 255;
                    if (B > 255) B = 255;

                    if (R < 0) R = 0;
                    if (G < 0) G = 0;
                    if (B < 0) B = 0;
                    color = Color.FromArgb(R,G,B);
                    brushY.Color = color;
                    gY.FillRectangle(brushY, i, j, 1, 1);
                }
            }
            Graphics gCb = pictureBox2.CreateGraphics();
            SolidBrush brushCb = new SolidBrush(Color.Black);
            gCb.Clear(BackColor);
            for (int i = 0; i < pictureBox2.Height; ++i)
            {
                for (int j = 0; j < pictureBox2.Width; ++j)
                {
                    int R = (int)Y[i, j];
                    int G = (int)Cb[i, j];
                    int B = (int)Y[i, j];

                    if (R > 255) R = 255;
                    if (G > 255) G = 255;
                    if (B > 255) B = 255;

                    if (R < 0) R = 0;
                    if (G < 0) G = 0;
                    if (B < 0) B = 0;
                    color = Color.FromArgb(R, G, B);
                    brushCb.Color = color;
                    gCb.FillRectangle(brushCb, i, j, 1, 1);
                }
            }
            Graphics gCr = pictureBox3.CreateGraphics();
            SolidBrush brushCr = new SolidBrush(Color.Black);
            gCr.Clear(BackColor);
            for (int i = 0; i < pictureBox3.Height; ++i)
            {
                for (int j = 0; j < pictureBox3.Width; ++j)
                {
                    int R = (int)Y[i, j];
                    int G = (int)Y[i, j];
                    int B = (int)Cr[i, j];

                    if (R > 255) R = 255;
                    if (G > 255) G = 255;
                    if (B > 255) B = 255;

                    if (R < 0) R = 0;
                    if (G < 0) G = 0;
                    if (B < 0) B = 0;
                    color = Color.FromArgb(R, G, B);
                    brushCr.Color = color;
                    gCr.FillRectangle(brushCr, i, j, 1, 1);
                }
            }

            Graphics gY1 = pictureBox4.CreateGraphics();
            SolidBrush brushY1 = new SolidBrush(Color.Black);
            gY1.Clear(BackColor);
            for (int i = 0; i < pictureBox4.Height; ++i)
            {
                for (int j = 0; j < pictureBox4.Width; ++j)
                {
                    int R = (int)newY[i, j];
                    int G = (int)newY[i, j];
                    int B = (int)newY[i, j];

                    if (R > 255) R = 255;
                    if (G > 255) G = 255;
                    if (B > 255) B = 255;

                    if (R < 0) R = 0;
                    if (G < 0) G = 0;
                    if (B < 0) B = 0;
                    color = Color.FromArgb(R, G, B);
                    brushY1.Color = color;
                    gY1.FillRectangle(brushY1, i, j, 1, 1);
                }
            }
            Graphics gCb1 = pictureBox5.CreateGraphics();
            SolidBrush brushCb1 = new SolidBrush(Color.Black);
            gCb1.Clear(BackColor);
            for (int i = 0; i < pictureBox5.Height; ++i)
            {
                for (int j = 0; j < pictureBox5.Width; ++j)
                {
                    int R = (int)newY[i, j];
                    int G = (int)newCb[i / N, j / N];
                    int B = (int)newY[i, j];

                    if (R > 255) R = 255;
                    if (G > 255) G = 255;
                    if (B > 255) B = 255;

                    if (R < 0) R = 0;
                    if (G < 0) G = 0;
                    if (B < 0) B = 0;
                    color = Color.FromArgb(R, G, B);
                    brushCb1.Color = color;
                    gCb1.FillRectangle(brushCb1, i, j, 1, 1);
                }
            }
            Graphics gCr1 = pictureBox6.CreateGraphics();
            SolidBrush brushCr1 = new SolidBrush(Color.Black);
            gCr1.Clear(BackColor);
            for (int i = 0; i < pictureBox6.Height; ++i)
            {
                for (int j = 0; j < pictureBox6.Width; ++j)
                {
                    int R = (int)newY[i, j];
                    int G = (int)newY[i, j];
                    int B = (int)newCr[i / N, j / N];

                    if (R > 255) R = 255;
                    if (G > 255) G = 255;
                    if (B > 255) B = 255;

                    if (R < 0) R = 0;
                    if (G < 0) G = 0;
                    if (B < 0) B = 0;
                    color = Color.FromArgb(R, G, B);
                    brushCr1.Color = color;
                    gCr1.FillRectangle(brushCr1, i, j, 1, 1);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
