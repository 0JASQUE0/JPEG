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
        public int size;
        public RGB(Bitmap bitmap1, Bitmap bitmap2, int Size)
        {
            image = bitmap1;
            result = bitmap2;
            size = Size;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap img1 = new Bitmap(image);
            Bitmap img2 = new Bitmap(image);
            Bitmap img3 = new Bitmap(image);
            Bitmap img4 = new Bitmap(image);
            Bitmap img5 = new Bitmap(image);
            Bitmap img6 = new Bitmap(image);
            //Исходное изображение

            pictureBox1.Image = null;
            for (int i = 0; i < size; ++i)            
                for (int j = 0; j < size; ++j)                
                    img1.SetPixel(i, j, Color.FromArgb(image.GetPixel(i, j).R, 0, 0));                
            
            pictureBox1.Image = img1;


            pictureBox2.Image = null;
            for (int i = 0; i < size; ++i)            
                for (int j = 0; j < size; ++j)                
                    img2.SetPixel(i, j, Color.FromArgb(0, image.GetPixel(i, j).G, 0));

            pictureBox2.Image = img2;

            pictureBox3.Image = null;
            for (int i = 0; i < size; ++i)            
                for (int j = 0; j < size; ++j)                
                    img3.SetPixel(i, j, Color.FromArgb(0, 0, image.GetPixel(i, j).B));

            pictureBox3.Image = img3;

            //Обработанное изображение
            pictureBox4.Image = null;
            for (int i = 0; i < size; ++i)            
                for (int j = 0; j < size; ++j)                
                    img4.SetPixel(i, j, Color.FromArgb(result.GetPixel(i, j).R, 0, 0));

            pictureBox4.Image = img4;

            pictureBox5.Image = null;
            for (int i = 0; i < size; ++i)            
                for (int j = 0; j < size; ++j)                
                    img5.SetPixel(i, j, Color.FromArgb(0, result.GetPixel(i, j).G, 0));

            pictureBox5.Image = img5;

            pictureBox6.Image = null;
            for (int i = 0; i < size; ++i)            
                for (int j = 0; j < size; ++j)                
                    img6.SetPixel(i, j, Color.FromArgb(0, 0, result.GetPixel(i, j).B));

            pictureBox6.Image = img6;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
