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
        public int size;

        public YCbCr(float[,] Y1, float[,] Cb1, float[,] Cr1, float[,] Y2, float[,] Cb2, float[,] Cr2, int n, int Size)
        {
            Y = Y1;
            Cb = Cb1;
            Cr = Cr1;
            newY = Y2;
            newCb = Cb2;
            newCr = Cr2;
            N = n;
            size = Size;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap img1 = new Bitmap(size, size);
            Bitmap img2 = new Bitmap(size, size);
            Bitmap img3 = new Bitmap(size, size);
            Bitmap img4 = new Bitmap(size, size);
            Bitmap img5 = new Bitmap(size, size);
            Bitmap img6 = new Bitmap(size, size);

            //Исходное изображение          
            pictureBox1.Image = null;
            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
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
                    img1.SetPixel(i, j, Color.FromArgb(R, G, B));
                }
            }
            pictureBox1.Image = img1;

            pictureBox2.Image = null;
            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    int B = (int)((255 - Cb[i, j]) * 0 + Cb[i, j] * 1);
                    int G = (int)((255 - Cb[i, j]) * 1 + Cb[i, j] * 0);
                    int R = (int)((255 - Cb[i, j]) * 0.5 + Cb[i, j] * 0.5);

                    if (R > 255) R = 255;
                    if (G > 255) G = 255;
                    if (B > 255) B = 255;

                    if (R < 0) R = 0;
                    if (G < 0) G = 0;
                    if (B < 0) B = 0;
                    img2.SetPixel(i, j, Color.FromArgb(R, G, B));
                }
            }
            pictureBox2.Image = img2;

            pictureBox3.Image = null;
            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    int B = (int)((255 - Cr[i, j]) * 0.5 + Cr[i, j] * 0.5);
                    int G = (int)((255 - Cr[i, j]) * 1 + Cr[i, j] * 0);
                    int R = (int)((255 - Cr[i, j]) * 0 + Cr[i, j] * 1);

                    if (R > 255) R = 255;
                    if (G > 255) G = 255;
                    if (B > 255) B = 255;

                    if (R < 0) R = 0;
                    if (G < 0) G = 0;
                    if (B < 0) B = 0;
                    img3.SetPixel(i, j, Color.FromArgb(R, G, B));
                }
            }
            pictureBox3.Image = img3;

            pictureBox4.Image = null;
            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
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
                    img4.SetPixel(i, j, Color.FromArgb(R, G, B));
                }
            }
            pictureBox4.Image = img4;

            pictureBox5.Image = null;
            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    int B = (int)((255 - newCb[i / N, j / N]) * 0 + newCb[i / N, j / N] * 1);
                    int G = (int)((255 - newCb[i / N, j / N]) * 1 + newCb[i / N, j / N] * 0);
                    int R = (int)((255 - newCb[i / N, j / N]) * 0.5 + newCb[i / N, j / N] * 0.5);

                    if (R > 255) R = 255;
                    if (G > 255) G = 255;
                    if (B > 255) B = 255;

                    if (R < 0) R = 0;
                    if (G < 0) G = 0;
                    if (B < 0) B = 0;
                    img5.SetPixel(i, j, Color.FromArgb(R, G, B));
                }
            }
            pictureBox5.Image = img5;

            pictureBox6.Image = null;
            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    int B = (int)((255 - newCr[i / N, j / N]) * 0.5 + newCr[i / N, j / N] * 0.5);
                    int G = (int)((255 - newCr[i / N, j / N]) * 1 + newCr[i / N, j / N] * 0);
                    int R = (int)((255 - newCr[i / N, j / N]) * 0 + newCr[i / N, j / N] * 1);

                    if (R > 255) R = 255;
                    if (G > 255) G = 255;
                    if (B > 255) B = 255;

                    if (R < 0) R = 0;
                    if (G < 0) G = 0;
                    if (B < 0) B = 0;
                    img6.SetPixel(i, j, Color.FromArgb(R, G, B));
                }
            }
            pictureBox6.Image = img6;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
