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
    public partial class RGB : Form
    {
        public Bitmap image;
        public Bitmap result;
        public RGB(Bitmap bitmap1, Bitmap bitmap2)
        {
            image = bitmap1;
            result = bitmap2;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Исходное изображение
            Color color;
            Graphics gR = pictureBox1.CreateGraphics();
            SolidBrush brushR = new SolidBrush(Color.Black);
            gR.Clear(BackColor);
            for (int i = 0; i < pictureBox1.Height; ++i)
            {
                for (int j = 0; j < pictureBox1.Width; ++j)
                {
                    color = image.GetPixel(i, j);
                    brushR.Color = Color.FromArgb(color.R, 0, 0);
                    gR.FillRectangle(brushR, i, j, 1, 1);
                }
            }
            Graphics gG = pictureBox2.CreateGraphics();
            SolidBrush brushG = new SolidBrush(Color.Black);
            gG.Clear(BackColor);
            for (int i = 0; i < pictureBox2.Height; ++i)
            {
                for (int j = 0; j < pictureBox2.Width; ++j)
                {
                    color = image.GetPixel(i, j);
                    brushG.Color = Color.FromArgb(0, color.G, 0);
                    gG.FillRectangle(brushG, i, j, 1, 1);
                }
            }
            Graphics gB = pictureBox3.CreateGraphics();
            SolidBrush brushB = new SolidBrush(Color.Black);
            gB.Clear(BackColor);
            for (int i = 0; i < pictureBox3.Height; ++i)
            {
                for (int j = 0; j < pictureBox3.Width; ++j)
                {
                    color = image.GetPixel(i, j);
                    brushB.Color = Color.FromArgb(0, 0, color.B);
                    gB.FillRectangle(brushB, i, j, 1, 1);
                }
            }

            //Обработанное изображение
            Graphics gR1 = pictureBox4.CreateGraphics();
            SolidBrush brushR1 = new SolidBrush(Color.Black);
            gR1.Clear(BackColor);
            for (int i = 0; i < pictureBox4.Height; ++i)
            {
                for (int j = 0; j < pictureBox4.Width; ++j)
                {
                    color = result.GetPixel(i, j);
                    brushR1.Color = Color.FromArgb(color.R, 0, 0);
                    gR1.FillRectangle(brushR1, i, j, 1, 1);
                }
            }
            Graphics gG1 = pictureBox5.CreateGraphics();
            SolidBrush brushG1 = new SolidBrush(Color.Black);
            gG1.Clear(BackColor);
            for (int i = 0; i < pictureBox5.Height; ++i)
            {
                for (int j = 0; j < pictureBox5.Width; ++j)
                {
                    color = result.GetPixel(i, j);
                    brushG1.Color = Color.FromArgb(0, color.G, 0);
                    gG1.FillRectangle(brushG1, i, j, 1, 1);
                }
            }
            Graphics gB1 = pictureBox6.CreateGraphics();
            SolidBrush brushB1 = new SolidBrush(Color.Black);
            gB1.Clear(BackColor);
            for (int i = 0; i < pictureBox6.Height; ++i)
            {
                for (int j = 0; j < pictureBox6.Width; ++j)
                {
                    color = result.GetPixel(i, j);
                    brushB1.Color = Color.FromArgb(0, 0, color.B);
                    gB1.FillRectangle(brushB1, i, j, 1, 1);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
