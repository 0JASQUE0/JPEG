﻿using System;
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
    public partial class Form1 : Form
    {
        private Bitmap image;

        float[,] Y;
        float[,] Cb;
        float[,] Cr;
        // Коэффициент сжатия CbCr
        int n;
        // Размер ДКП
        int N;
        // Коэффииент сжатия
        int Q;

        public Form1()
        {
            InitializeComponent();
        }

        private int[,] MatrixMultiplication(int[,] matrix1, float[,] matrix2)
        {
            if (matrix1.GetUpperBound(1) != matrix2.GetUpperBound(0))
                throw new Exception("These matrices cannot be multiplied");
            int[,] result = new int[matrix1.GetUpperBound(0) + 1, matrix2.GetUpperBound(1) + 1];
            for (int i = 0; i < matrix1.GetUpperBound(0) + 1; ++i)
                for (int j = 0; j < result.GetUpperBound(1) + 1; ++j)
                    for (int k = 0; k < matrix1.GetUpperBound(1) + 1; ++k)
                        result[i, j] += (int)(matrix1[i, k] * matrix2[k, j]);
            return result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog open_dialog = new OpenFileDialog(); //создание диалогового окна для выбора файла
            open_dialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*"; //формат загружаемого файла
            if (open_dialog.ShowDialog() == DialogResult.OK) //если в окне была нажата кнопка "ОК"
            {
                try
                {
                    image = new Bitmap(open_dialog.FileName);
                    pictureBox1.Image = image;
                }
                catch
                {
                    DialogResult result = MessageBox.Show("Невозможно открыть выбранный файл",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            n = 2;

            Y = new float[pictureBox1.Width, pictureBox1.Height];
            Cb = new float[pictureBox1.Width, pictureBox1.Height];
            Cr = new float[pictureBox1.Width, pictureBox1.Height];

            YCbCrColor conv = new YCbCrColor();
            Color color;

            //image = new Bitmap(pictureBox1.Image);

            // RBG to YCbCr
            for (int i = 0; i < pictureBox1.Height; ++i)
            {
                for (int j = 0; j < pictureBox1.Width; ++j)
                {
                    color = image.GetPixel(i, j);
                    conv = conv.FromRgbColor(color);
                    Y[i, j] = conv.Y;
                    Cb[i, j] = conv.Cb;
                    Cr[i, j] = conv.Cr;
                }
            }

            // Прореживание
            float[,] newCb = new float[pictureBox1.Width / n, pictureBox1.Height / n];
            float[,] newCr = new float[pictureBox1.Width / n, pictureBox1.Height / n];

            for (int i = 0; i < pictureBox1.Height / n; ++i)
            {
                for (int j = 0; j < pictureBox1.Width / n; ++j)
                {
                    newCb[i, j] += (Cb[i * n, j * n] + Cb[i * n, j * n + 1] + Cb[i * n + 1, j * n] + Cb[i * n + 1, j * n + 1]) / 4;
                    newCr[i, j] += (Cr[i * n, j * n] + Cr[i * n, j * n + 1] + Cr[i * n + 1, j * n] + Cr[i * n + 1, j * n + 1]) / 4;
                }
            }

            // Матрица ДКП
            N = 8;
            float[,] dct = new float[N, N];
            for (int i = 0; i < N; ++i)
            {
                for (int j = 0; j < N; ++j)
                {
                    if (i == 0) dct[i, j] = (float)(1 / Math.Sqrt(N));
                    else dct[i, j] = (float)(Math.Sqrt(2.0 / N) * Math.Cos(((2 * j + 1) * i * Math.PI) / (2 * N)));
                }
            }

            //label1.Text = (image.Height * image.Width * 24).ToString();

            for (int ii = 0; ii < 32; ++ii)
            {
                for (int jj = 0; jj < 32; ++jj)
                {

                    int[,] test = new int[8, 8]
                    {
                        { (int)Y[8 * ii, 8 * jj], (int)Y[8 * ii, 8 * jj + 1], (int)Y[8 * ii, 8 * jj + 2], (int)Y[8 * ii, 8 * jj + 3], (int)Y[8 * ii, 8 * jj + 4], (int)Y[8 * ii, 8 * jj + 5], (int)Y[8 * ii, 8 * jj + 6], (int)Y[8 * ii, 8 * jj + 7] },
                        { (int)Y[1 + 8 * ii, 8 * jj], (int)Y[1 + 8 * ii, 8 * jj + 1], (int)Y[1 + 8 * ii, 8 * jj + 2], (int)Y[1 + 8 * ii, 8 * jj + 3], (int)Y[1 + 8 * ii, 8 * jj + 4], (int)Y[1 + 8 * ii, 8 * jj + 5], (int)Y[1 + 8 * ii, 8 * jj + 6], (int)Y[1 + 8 * ii, 8 * jj + 7] },
                        { (int)Y[2 + 8 * ii, 8 * jj], (int)Y[2 + 8 * ii, 8 * jj + 1], (int)Y[2 + 8 * ii, 8 * jj + 2], (int)Y[2 + 8 * ii, 8 * jj + 3], (int)Y[2 + 8 * ii, 8 * jj + 4], (int)Y[2 + 8 * ii, 8 * jj + 5], (int)Y[2 + 8 * ii, 8 * jj + 6], (int)Y[2 + 8 * ii, 8 * jj + 7] },
                        { (int)Y[3 + 8 * ii, 8 * jj], (int)Y[3 + 8 * ii, 8 * jj + 1], (int)Y[3 + 8 * ii, 8 * jj + 2], (int)Y[3 + 8 * ii, 8 * jj + 3], (int)Y[3 + 8 * ii, 8 * jj + 4], (int)Y[3 + 8 * ii, 8 * jj + 5], (int)Y[3 + 8 * ii, 8 * jj + 6], (int)Y[3 + 8 * ii, 8 * jj + 7] },
                        { (int)Y[4 + 8 * ii, 8 * jj], (int)Y[4 + 8 * ii, 8 * jj + 1], (int)Y[4 + 8 * ii, 8 * jj + 2], (int)Y[4 + 8 * ii, 8 * jj + 3], (int)Y[4 + 8 * ii, 8 * jj + 4], (int)Y[4 + 8 * ii, 8 * jj + 5], (int)Y[4 + 8 * ii, 8 * jj + 6], (int)Y[4 + 8 * ii, 8 * jj + 7] },
                        { (int)Y[5 + 8 * ii, 8 * jj], (int)Y[5 + 8 * ii, 8 * jj + 1], (int)Y[5 + 8 * ii, 8 * jj + 2], (int)Y[5 + 8 * ii, 8 * jj + 3], (int)Y[5 + 8 * ii, 8 * jj + 4], (int)Y[5 + 8 * ii, 8 * jj + 5], (int)Y[5 + 8 * ii, 8 * jj + 6], (int)Y[5 + 8 * ii, 8 * jj + 7] },
                        { (int)Y[6 + 8 * ii, 8 * jj], (int)Y[6 + 8 * ii, 8 * jj + 1], (int)Y[6 + 8 * ii, 8 * jj + 2], (int)Y[6 + 8 * ii, 8 * jj + 3], (int)Y[6 + 8 * ii, 8 * jj + 4], (int)Y[6 + 8 * ii, 8 * jj + 5], (int)Y[6 + 8 * ii, 8 * jj + 6], (int)Y[6 + 8 * ii, 8 * jj + 7] },
                        { (int)Y[7 + 8 * ii, 8 * jj], (int)Y[7 + 8 * ii, 8 * jj + 1], (int)Y[7 + 8 * ii, 8 * jj + 2], (int)Y[7 + 8 * ii, 8 * jj + 3], (int)Y[7 + 8 * ii, 8 * jj + 4], (int)Y[7 + 8 * ii, 8 * jj + 5], (int)Y[7 + 8 * ii, 8 * jj + 6], (int)Y[7 + 8 * ii, 8 * jj + 7] },
                    };

                    // Усреднение яркости
                    for (int i = 0; i < N; ++i)
                    {
                        for (int j = 0; j < N; ++j)
                        {
                            test[i, j] -= 128;
                        }
                    }

                    // Умножение на матрицу ДКП
                    test = MatrixMultiplication(test, dct);

                    // Квантование
                    Q = 2;
                    int[,] q = new int[N, N];

                    for (int i = 0; i < N; ++i)
                    {
                        for (int j = 0; j < N; ++j)
                        {
                            q[i, j] = 1 + ((1 + i + j) * Q);
                        }
                    }

                    for (int i = 0; i < N; ++i)
                    {
                        for (int j = 0; j < N; ++j)
                        {
                            test[i, j] /= q[i, j];
                        }
                    }

                    /*// Прохождение зигзагом
                    int[] t = new int[64];

                    int I = 0;
                    int J = 0;

                    int m = 0;
                    t[m] = test[I, J];

                    while (I != N - 1 || J != N - 1)
                    {
                        // 1, подняться по диагонали вверх
                        while (I - 1 >= 0 && J + 1 <= N - 1)
                        {
                            t[++m] = test[--I, ++J];
                        }
                        // 2, сдвиньте один шаг вправо, если вы не можете двигаться вправо, сдвиньте один шаг вниз
                        if (J + 1 <= N - 1)
                        {
                            t[++m] = test[I, ++J];
                        }
                        else if (I + 1 <= N - 1)
                        {
                            t[++m] = test[++I, J];
                        }
                        // 3, идти по диагонали до конца
                        while (I + 1 <= N - 1 && J - 1 >= 0)
                        {
                            t[++m] = test[++I, --J];
                        }
                        // 4. Двигайтесь вниз на один шаг, если вы не можете двигаться вниз, на один шаг вправо
                        if (I + 1 <= N - 1)
                        {
                            t[++m] = test[++I, J];
                        }
                        else if (J + 1 <= N - 1)
                        {
                            t[++m] = test[I,++J];
                        }
                    }*/




                    // Обратное квантование

                    for (int i = 0; i < N; ++i)
                    {
                        for (int j = 0; j < N; ++j)
                        {
                            test[i, j] *= q[i, j];
                        }
                    }

                    // Обратная матрица ДКП
                    float[,] idct = new float[8, 8]
                    {
                        { 0.3539f, 0.4905f, 0.4616f, 0.4155f, 0.3539f, 0.2782f, 0.1912f, 0.0971f },
                        { 0.3539f, 0.4158f, 0.1910f, -0.0977f, -0.3533f, -0.4901f, -0.4621f, -0.2782f },
                        { 0.3538f, 0.2779f, -0.1916f, 0.4906f, -0.3534f, 0.0977f, 0.4619f, 0.4155f },
                        { 0.3537f, 0.0975f, -0.4621f, -0.2779f, 0.3537f, 0.4158f, -0.1914f, -0.4905f },
                        { 0.3535f, -0.0975f, -0.4619f, 0.2779f, 0.3535f, -0.4158f, -0.1913f, 0.4905f },
                        { 0.3534f, -0.2779f, -0.1911f, 0.4906f, -0.3538f, -0.0977f, 0.4621f, -0.4155f },
                        { 0.3533f, -0.4158f, 0.1917f, 0.0977f, -0.3539f, 0.4901f, -0.4619f, 0.2782f },
                        { 0.3533f, -0.4905f, 0.4624f, -0.4155f, 0.3533f, -0.2782f, 0.1915f, -0.0971f }
                    };

                    // Умножение на обратную матрицу ДКП
                    test = MatrixMultiplication(test, idct);

                    // Обратное усреднение
                    for (int i = 0; i < N; ++i)
                    {
                        for (int j = 0; j < N; ++j)
                        {
                            test[i, j] += 128;
                        }
                    }

                    // Запись значений
                    for (int i = 0; i < N; ++i)
                    {
                        for (int j = 0; j < N; ++j)
                        {
                            Y[i + 8 * ii, 8 * jj + j] = test[i, j];
                        }
                    }

                }
            }

            /*Bitmap result = new Bitmap(image.Width, image.Height);
            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    color = image.GetPixel(i, j);
                    conv = conv.FromRgbColor(color);
                    result.SetPixel(i, j, conv.ToRgbColor());
                }
                pictureBox2.Image = result;
            }*/

            Bitmap result = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            for (int i = 0; i < pictureBox2.Height; i++)
            {
                for (int j = 0; j < pictureBox2.Width; j++)
                {
                    conv.Y = Y[i, j];
                    conv.Cb = newCb[i / n, j / n];
                    conv.Cr = newCr[i / n, j / n];
                    result.SetPixel(i, j, conv.ToRgbColor());
                }
                pictureBox2.Image = result;
            }

            result.Save("D:\\4 курс\\Программирование на кристалле\\pctr.png");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}