using System;
using System.Collections.Generic;
using System.Collections;
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
        private Bitmap img;
        private Bitmap result;

        // Коэффициент сжатия CbCr
        int n;
        // Размер ДКП
        int N;
        // Коэффииент сжатия
        int Q;

        float[,] Y;
        float[,] Cb;
        float[,] Cr;

        float[,] newCb;
        float[,] newCr;
        float[,] newY;

        List<KeyValuePair<int, int>> RLEList;
        List<int> numberOfElement;

        private Point RectStartPoint;
        private Rectangle Rect = new Rectangle();
        private Brush selectionBrush = new SolidBrush(Color.FromArgb(128, 72, 145, 220));

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

        private int[,] MatrixMultiplication(float[,] matrix1, int[,] matrix2)
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

        public float[,] GetMinor(float[,] matrix, int row, int column)
        {
            if (matrix.GetLength(0) != matrix.GetLength(1)) throw new Exception(" Число строк в матрице не совпадает с числом столбцов");
            float[,] buf = new float[matrix.GetLength(0) - 1, matrix.GetLength(0) - 1];
            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if ((i != row) || (j != column))
                    {
                        if (i > row && j < column) buf[i - 1, j] = matrix[i, j];
                        if (i < row && j > column) buf[i, j - 1] = matrix[i, j];
                        if (i > row && j > column) buf[i - 1, j - 1] = matrix[i, j];
                        if (i < row && j < column) buf[i, j] = matrix[i, j];
                    }
                }
            return buf;
        }

        public float Determ(float[,] matrix)
        {
            if (matrix.GetLength(0) != matrix.GetLength(1)) throw new Exception(" Число строк в матрице не совпадает с числом столбцов");
            float det = 0;
            int Rank = matrix.GetLength(0);
            if (Rank == 1) det = matrix[0, 0];
            if (Rank == 2) det = matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
            if (Rank > 2)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    det += (float)(Math.Pow(-1, 0 + j) * matrix[0, j] * Determ(GetMinor(matrix, 0, j)));
                }
            }
            return det;
        }

        float Convert(float value, float From1, float From2, float To1, float To2)
        {
            return (value - From1) / (From2 - From1) * (To2 - To1) + To1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog open_dialog = new OpenFileDialog
            {
                Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*" //формат загружаемого файла
            }; //создание диалогового окна для выбора файла
            if (open_dialog.ShowDialog() == DialogResult.OK) //если в окне была нажата кнопка "ОК"
            {
                try
                {
                    image = new Bitmap(open_dialog.FileName);
                    image = new Bitmap(image, 256, 256);
                    pictureBox1.Image = image;
                }
                catch
                {
                    DialogResult result = MessageBox.Show("Невозможно открыть выбранный файл",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            Rect = new Rectangle();
            //Rect.Location = new Point(0, 0);
            pictureBox1.Image = image;
            pictureBox2.Image = null;
            pictureBox3.Image = null;
            pictureBox4.Image = null;
            pictureBox5.Image = null;
            pictureBox6.Image = null;
            pictureBox7.Image = null;
            pictureBox8.Image = null;
            pictureBox9.Image = null;
            pictureBox10.Image = null;
            pictureBox11.Image = null;
            pictureBox12.Image = null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int size = Rect.Size.Width;
            if (size % 8 != 0)
                size = (size / 8) * 8 + 8;
            Rect.Size = new Size(size, size);

            Y = new float[pictureBox1.Width, pictureBox1.Height];
            Cb = new float[pictureBox1.Width, pictureBox1.Height];
            Cr = new float[pictureBox1.Width, pictureBox1.Height];

            YCbCrColor conv = new YCbCrColor();
            Color color;

            int[] gistR = new int[256];
            int[] gistG = new int[256];
            int[] gistB = new int[256];
            int[] gistY = new int[256];

            int[] gistNewR = new int[256];
            int[] gistNewG = new int[256];
            int[] gistNewB = new int[256];
            int[] gistNewY = new int[256];



            if (Rect.Location.X != 0 && Rect.Location.Y != 0)
            {
                img = new Bitmap(image, Rect.Width, Rect.Height);
                for (int i = Rect.Location.X; i < Rect.Location.X + Rect.Width; ++i)
                    for (int j = Rect.Location.Y; j < Rect.Location.Y + Rect.Height; ++j)
                        img.SetPixel(i - Rect.Location.X, j - Rect.Location.Y, image.GetPixel(i, j));

                pictureBox8.Image = img;
            }
            else
                img = image;

            int sizeOfImage = img.Size.Width;

            // RBG to YCbCr
            for (int i = 0; i < sizeOfImage; ++i)
            {
                for (int j = 0; j < sizeOfImage; ++j)
                {
                    color = img.GetPixel(i, j);
                    conv = conv.FromRgbColor(color);
                    Y[i, j] = conv.Y;
                    Cb[i, j] = conv.Cb;
                    Cr[i, j] = conv.Cr;
                }
            }

            // Прореживание
            n = Int32.Parse(comboBox1.Text);
            newCb = new float[sizeOfImage / n, sizeOfImage / n];
            newCr = new float[sizeOfImage / n, sizeOfImage / n];
            newY = new float[sizeOfImage, sizeOfImage];

            for (int i = 0; i < sizeOfImage / n; ++i)
            {
                for (int j = 0; j < sizeOfImage / n; ++j)
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

            // Обратная матрица ДКП
            float[,] idct = new float[N, N];
            float det = Determ(dct);
            for (int i = 0; i < N; ++i)
            {
                for (int j = 0; j < N; ++j)
                {
                    idct[i, j] = dct[j, i];
                    //idct[i, j] = (float)((1.0 / det) * dct[j, i]);
                }
            }

            // Матрица квантования
            Q = Int32.Parse(textBox1.Text);
            int[,] q = new int[N, N];

            for (int i = 0; i < N; ++i)
            {
                for (int j = 0; j < N; ++j)
                {
                    q[i, j] = 1 + ((1 + i + j) * Q);
                }
            }

            RLEList = new List<KeyValuePair<int, int>>();
            numberOfElement = new List<int>();

            // Работа с блоками 8x8
            for (int ii = 0; ii < sizeOfImage / 8; ++ii)
            {
                for (int jj = 0; jj < sizeOfImage / 8; ++jj)
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
                    test = MatrixMultiplication(dct, MatrixMultiplication(test, idct));

                    // Квантование                    
                    for (int i = 0; i < N; ++i)
                    {
                        for (int j = 0; j < N; ++j)
                        {
                            test[i, j] /= q[i, j];
                        }
                    }

                    // Прохождение зигзагом
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
                            t[++m] = test[I, ++J];
                        }
                    }

                    // RLE
                    int counter = 0;
                    KeyValuePair<int, int> temp;

                    for (int i = 0; i < t.Length; ++i)
                    {
                        if (t[i] == 0)
                            counter++;
                        else
                        {
                            temp = new KeyValuePair<int, int>(counter, t[i]);
                            counter = 0;
                            if (!RLEList.Contains(temp))
                            {
                                RLEList.Add(temp);
                                numberOfElement.Add(1);
                            }
                            else
                            {
                                numberOfElement[RLEList.FindIndex(p => p.Key == temp.Key && p.Value == temp.Value)]++;
                                RLEList.Add(temp);
                                numberOfElement.Add(0);
                            }
                        }
                    }
                    temp = new KeyValuePair<int, int>(-1, -1);
                    if (!RLEList.Contains(temp))
                    {
                        RLEList.Add(temp);
                        numberOfElement.Add(1);
                    }
                    else
                    {
                        numberOfElement[RLEList.FindIndex(p => p.Key == temp.Key && p.Value == temp.Value)]++;
                        RLEList.Add(temp);
                        numberOfElement.Add(0);
                    }

                    // Обратное квантование
                    for (int i = 0; i < N; ++i)
                    {
                        for (int j = 0; j < N; ++j)
                        {
                            test[i, j] *= q[i, j];
                        }
                    }
                    
                    test = MatrixMultiplication(idct, MatrixMultiplication(test, dct));

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
                            newY[i + 8 * ii, 8 * jj + j] = test[i, j];
                        }
                    }
                }
            }

            // Кодирование методом Хаффмана
            HuffmanTree huffmanTree = new HuffmanTree();
            huffmanTree.Build(RLEList, numberOfElement);
            BitArray encoded = huffmanTree.Encode(RLEList);

            label9.Text = "Коэффициент сжатия: " + (float)((sizeOfImage * sizeOfImage) / (encoded.Length / 8.0));
            //label9.Text = "Коэффициент сжатия: " + ((sizeOfImage * sizeOfImage * 3) / (encoded.Length / 8 + 2 * ((float)sizeOfImage / n) * ((float)sizeOfImage / n)));

            // Получение данных для гистограммы яркости
            for (int i = 0; i < sizeOfImage; ++i)
            {
                for (int j = 0; j < sizeOfImage; ++j)
                {
                    if ((int)Y[i, j] > 255)
                        gistY[255]++;
                    else if ((int)Y[i, j] < 0)
                        gistY[0]++;
                    else
                        gistY[(int)Y[i, j]]++;

                    if ((int)newY[i, j] > 255)
                        gistNewY[255]++;
                    else if ((int)newY[i, j] < 0)
                        gistNewY[0]++;
                    else
                        gistNewY[(int)newY[i, j]]++;
                }
            }

            // Вывод изображения 
            result = new Bitmap(sizeOfImage, sizeOfImage);
            for (int i = 0; i < sizeOfImage; i++)
            {
                for (int j = 0; j < sizeOfImage; j++)
                {
                    conv.Y = newY[i, j];
                    conv.Cb = Cb[i, j];
                    conv.Cr = Cr[i, j];
                    result.SetPixel(i, j, conv.ToRgbColor());
                }
                pictureBox2.Image = result;
            }

            // Получение данных для гистограммы RGB
            for (int i = 0; i < sizeOfImage; ++i)
            {
                for (int j = 0; j < sizeOfImage; ++j)
                {
                    color = image.GetPixel(i, j);
                    gistR[color.R]++;
                    gistG[color.G]++;
                    gistB[color.B]++;

                    color = result.GetPixel(i, j);
                    gistNewR[color.R]++;
                    gistNewG[color.R]++;
                    gistNewB[color.R]++;
                }
            }

            // Рисование гистограмм
            Graphics gY = pictureBox3.CreateGraphics();
            Graphics gR = pictureBox4.CreateGraphics();
            Graphics gG = pictureBox5.CreateGraphics();
            Graphics gB = pictureBox6.CreateGraphics();
            Graphics gNewY = pictureBox12.CreateGraphics();
            Graphics gNewR = pictureBox11.CreateGraphics();
            Graphics gNewG = pictureBox10.CreateGraphics();
            Graphics gNewB = pictureBox9.CreateGraphics();
            gY.Clear(BackColor);
            gR.Clear(BackColor);
            gG.Clear(BackColor);
            gB.Clear(BackColor);
            gNewY.Clear(BackColor);
            gNewR.Clear(BackColor);
            gNewG.Clear(BackColor);
            gNewB.Clear(BackColor);
            for (int i = 0; i < 256; ++i)
            {
                gY.DrawLine(new Pen(Color.Black, 1), i, 256, i, pictureBox3.Height - Convert(gistY[i], 0, gistY.Max(), 0, 64));
                gR.DrawLine(new Pen(Color.Red, 1), i, 256, i, pictureBox4.Height - Convert(gistR[i], 0, gistR.Max(), 0, 64));
                gG.DrawLine(new Pen(Color.Green, 1), i, 256, i, pictureBox5.Height - Convert(gistG[i], 0, gistG.Max(), 0, 64));
                gB.DrawLine(new Pen(Color.Blue, 1), i, 256, i, pictureBox6.Height - Convert(gistB[i], 0, gistB.Max(), 0, 64));
                gNewY.DrawLine(new Pen(Color.Black, 1), i, 256, i, pictureBox12.Height - Convert(gistNewY[i], 0, gistY.Max(), 0, 64));
                gNewR.DrawLine(new Pen(Color.Red, 1), i, 256, i, pictureBox11.Height - Convert(gistNewR[i], 0, gistR.Max(), 0, 64));
                gNewG.DrawLine(new Pen(Color.Green, 1), i, 256, i, pictureBox10.Height - Convert(gistNewG[i], 0, gistG.Max(), 0, 64));
                gNewB.DrawLine(new Pen(Color.Blue, 1), i, 256, i, pictureBox9.Height - Convert(gistNewB[i], 0, gistB.Max(), 0, 64));
            }

            // Подсчет СКО
            double MSE = 0;
            for (int i = 0; i < sizeOfImage; ++i)
            {
                for (int j = 0; j < sizeOfImage; ++j)
                {
                    MSE += Math.Pow(Y[i, j] - newY[i, j], 2);
                }
            }
            MSE /= (double)(sizeOfImage * sizeOfImage);
            label6.Text = "СКО: " + Math.Sqrt(MSE).ToString();

            double PSNR = 10 * Math.Log10(255.0 / MSE);
            label10.Text = "Отношение сигнала к шуму: " + PSNR.ToString();

            // Вывод разности изображений
            Bitmap difference = new Bitmap(sizeOfImage, sizeOfImage);
            for (int i = 0; i < sizeOfImage; i++)
            {
                for (int j = 0; j < sizeOfImage; j++)
                {
                    int R = (int)(Y[i, j] - newY[i, j]);
                    int G = (int)(Y[i, j] - newY[i, j]);
                    int B = (int)(Y[i, j] - newY[i, j]);

                    if (R > 255) R = 255;
                    if (G > 255) G = 255;
                    if (B > 255) B = 255;

                    if (R < 0) R = 0;
                    if (G < 0) G = 0;
                    if (B < 0) B = 0;
                    color = Color.FromArgb(255 - R, 255 - G, 255 - B);
                    difference.SetPixel(i, j, color);
                }
                pictureBox7.Image = difference;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RGB rgb = new RGB(img, result, img.Height);
            rgb.ShowDialog(this);
            rgb.Dispose();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            YCbCr yCbCr = new YCbCr(Y, Cb, Cr, newY, newCb, newCr, n, img.Height);
            yCbCr.ShowDialog(this);
            yCbCr.Dispose();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            RectStartPoint = e.Location;
            Invalidate();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            Point tempEndPoint = e.Location;

            Rect.Location = new Point(Math.Min(RectStartPoint.X, tempEndPoint.X), Math.Min(RectStartPoint.Y, tempEndPoint.Y));

            int size;
            if (Math.Abs(RectStartPoint.X - tempEndPoint.X) > Math.Abs(RectStartPoint.Y - tempEndPoint.Y))
                size = Math.Abs(RectStartPoint.X - tempEndPoint.X);
            else
                size = Math.Abs(RectStartPoint.Y - tempEndPoint.Y);
            Rect.Size = new Size(size, size);

            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                if (Rect != null && Rect.Width > 0 && Rect.Height > 0)
                {
                    e.Graphics.FillRectangle(selectionBrush, Rect);
                }
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Rect.Contains(e.Location))
                {
                    MessageBox.Show("Error");
                }
            }
        }
    }
}
