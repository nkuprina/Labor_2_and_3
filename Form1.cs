﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace L
{
    public partial class Form1 : Form
    {
        bool drawing;
        int historyCounter; //Счетчик истории
        GraphicsPath currentPath;

        public Pen currentPen1 { get; private set; }

        Point oldLocation;
        public Pen currentPen;
        public Color historyColor;
        List<Image> History;

        public Color color = Form2.colorResult;

        public Form1()
        {
            InitializeComponent();
            drawing = false; //Переменная, ответственная за рисование
            currentPen = new Pen(Color.Black); //Инициализация пера с черным цветом
            currentPen.Width = trackBar1.Value;
            History = new List<Image>(); //Инициализация
            Data.Value = "111";

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentPen.Width = trackBar1.Value;
        }

  

        private void newToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            

            var result = MessageBox.Show("Сохранить текущее изображение перед созданием нового рисунка ? ", "Предупреждение", MessageBoxButtons.YesNoCancel);
                switch (result)
                {
                    case DialogResult.No: break;
                    case DialogResult.Yes: saveToolStripMenuItem_Click(sender, e); break;
                    case DialogResult.Cancel: return;
                }
            History.Clear();
            historyCounter = 0;
            Bitmap pic = new Bitmap(750, 500);
            picDrawingSurface.Image = pic;
            History.Add(new Bitmap(picDrawingSurface.Image));
            Graphics g = Graphics.FromImage(picDrawingSurface.Image);

            g.Clear(Color.White);
            g.DrawImage(picDrawingSurface.Image, 0, 0, 750, 500);
        }

        private void picDrawingSurface_MouseDown(object sender, MouseEventArgs e)
        {
            if (picDrawingSurface.Image == null)
            {
                MessageBox.Show("Сначала создайте новый файл!");
                return;
            }
            if (e.Button == MouseButtons.Left)
            {
                drawing = true;
                oldLocation = e.Location;
                currentPath = new GraphicsPath();
        
            }
            if (e.Button == MouseButtons.Right)
            {
                drawing = true;                     
                // Form2.colorResult
                currentPen = new Pen(Color.White, trackBar1.Value);
                oldLocation = e.Location;
                currentPath = new GraphicsPath();
            }


        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
            SaveFileDialog SaveDlg = new SaveFileDialog();
            SaveDlg.Filter = "JPEG Image|*.jpg|Bitmap Image|*.bmp|GIF Image|*.gif|PNGImage | *.png";
            SaveDlg.Title = "Save an Image File";
            SaveDlg.FilterIndex = 4;
            SaveDlg.ShowDialog();
            if (SaveDlg.FileName != "") //Если введено не пустое имя
            {
                System.IO.FileStream fs =
                (System.IO.FileStream)SaveDlg.OpenFile();
                switch (SaveDlg.FilterIndex)
                {
                    case 1:
                        this.picDrawingSurface.Image.Save(fs, ImageFormat.Jpeg);
                        break;
                    case 2:
                        this.picDrawingSurface.Image.Save(fs, ImageFormat.Bmp);
                        break;
                    case 3:
                        this.picDrawingSurface.Image.Save(fs, ImageFormat.Gif);
                        break;
                    case 4:
                        this.picDrawingSurface.Image.Save(fs, ImageFormat.Png);
                        break;
                }
                fs.Close();
            }

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog OP = new OpenFileDialog();
            OP.Filter = "JPEG Image|*.jpg|Bitmap Image|*.bmp|GIF Image|*.gif|PNG Image | *.png";
            OP.Title = "Open an Image File";
            OP.FilterIndex = 1; //По умолчанию будет выбрано первое расширение *.jpg И, когда пользователь укажет нужный путь к картинке, ее нужно будет загрузить в PictureBox:
            if (OP.ShowDialog() != DialogResult.Cancel)
            picDrawingSurface.Load(OP.FileName);
            picDrawingSurface.AutoSize = true;

        }

        private void picDrawingSurface_MouseUp(object sender, MouseEventArgs e)
        {
            //Очистка ненужной истории
            History.RemoveRange(historyCounter + 1, History.Count - historyCounter - 1);
            History.Add(new Bitmap(picDrawingSurface.Image));
            if (historyCounter + 1 < 10) historyCounter++;
            if (History.Count - 1 == 10) History.RemoveAt(0);
            drawing = false;
            try
            {
                currentPath.Dispose();
                Color historyColor = Form2.colorResult;
                currentPen = new Pen( historyColor, trackBar1.Value);                

            }
            catch { };
        }

        private void picDrawingSurface_MouseMove(object sender, MouseEventArgs e)
        {
            label1.Text = e.X.ToString() + ", " + e.Y.ToString();
            if (drawing)
            {
               
                Graphics g = Graphics.FromImage(picDrawingSurface.Image);
                currentPath.AddLine(oldLocation, e.Location);
                g.DrawPath(currentPen, currentPath);
                oldLocation = e.Location;
                g.Dispose();
                picDrawingSurface.Invalidate();
            }
           

        }

        private void andoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (History.Count != 0 && historyCounter != 0)
            {
                picDrawingSurface.Image = new Bitmap(History[--historyCounter]);
            }
            else MessageBox.Show("История пуста");

        }

        private void renoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (historyCounter < History.Count - 1)
            {
                picDrawingSurface.Image = new Bitmap(History[++historyCounter]);
            }
            else MessageBox.Show("История пуста");

        }

        private void solidToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPen.DashStyle = DashStyle.Solid;
            solidToolStripMenuItem.Checked = true;
            dotToolStripMenuItem.Checked = false;
            dushDotDotToolStripMenuItem.Checked = false;

        }

        private void dotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPen.DashStyle = DashStyle.Dot;

            solidToolStripMenuItem.Checked = false;
            dotToolStripMenuItem.Checked = true;
            dushDotDotToolStripMenuItem.Checked = false;
        }

        private void dushDotDotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPen.DashStyle = DashStyle.DashDotDot;
            solidToolStripMenuItem.Checked = false;
            dotToolStripMenuItem.Checked = false;
            dushDotDotToolStripMenuItem.Checked = true;
        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2(this.picDrawingSurface.Text, Form2.colorResult);
            f.ShowDialog();
            Data.Value = "111";
            currentPen = new Pen(Form2.colorResult, trackBar1.Value);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            newToolStripMenuItem1_Click(sender, e);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            openToolStripMenuItem_Click(sender, e);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            saveToolStripMenuItem_Click(sender, e);
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            colorToolStripMenuItem_Click(sender, e);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Графический редактор. Автор Natalja Kuprina Duarte");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

   
}
