using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace LW__17
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Boolean DropStarted = false;

        private Point origin = new Point(300, 380);
        int scale = 50;

        Region reg1 = new Region();
        Region reg2 = new Region();

        SolidBrush br3 = new SolidBrush(defaultBrush.Color);
        SolidBrush br4 = new SolidBrush(defaultBrush.Color);

        static SolidBrush hoverBrush = new SolidBrush(Color.Red);
        static SolidBrush defaultBrush = new SolidBrush(Color.Gray);

        Point locationX = new Point(535, 280);
        Point locationY = new Point(280, 30);
        Rectangle recX = new Rectangle(535, 380, 15, 20);
        Rectangle recY = new Rectangle(280, 130, 15, 20);
        Font drawFont = new Font("Times New Roman", 14);

        int activeElem = 0;
        bool growing = true;
        int rgb = 255;
        bool stop = false;

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics gr = e.Graphics;
            gr.DrawString("x", drawFont, Brushes.Black, recX);
            gr.DrawString("y", drawFont, Brushes.Black, recY);
        
            // Ось X
            gr.DrawLine(new Pen(Color.Black, 2), new Point(origin.X - 5 * scale, origin.Y), new Point(origin.X + 5 * scale, origin.Y));
            // Ось Y
            gr.DrawLine(new Pen(Color.Black, 2), new Point(origin.X, origin.Y + 5 * scale), new Point(origin.X, origin.Y - 5 * scale));

            // Вспомогательные линии
            //gr.DrawLine(new Pen(Color.Black, 2), new Point(origin.X + 3 * scale, origin.Y - 3 * scale), new Point(origin.X - 3 * scale, origin.Y + 3 * scale));
            //gr.DrawLine(new Pen(Color.Red, 1), new Point(origin.X, origin.Y - 2 * scale), new Point(origin.X + 2 * scale, origin.Y - 2 * scale));
            //gr.DrawLine(new Pen(Color.Red, 1), new Point(origin.X + 2 * scale, origin.Y), new Point(origin.X + 2 * scale, origin.Y - 2 * scale));

            // Окружности
            Rectangle rectCircle1 = new Rectangle(origin.X - 2 * scale, origin.Y - 2 * scale, 4 * scale, 4 * scale);
            gr.DrawEllipse(new Pen(Color.Black, 2), rectCircle1);
            Rectangle rectCircle2 = new Rectangle(origin.X, origin.Y - scale, 2 * scale, 2 * scale);
            gr.DrawEllipse(new Pen(Color.Black, 2), rectCircle2);

            // Закрашенные области
            GraphicsPath gpCircle1 = new GraphicsPath(); // Окружность БОЛЬШАЯ
            gpCircle1.AddEllipse(rectCircle1);
            GraphicsPath gpCircle2 = new GraphicsPath(); // Окружность МАЛАЯ
            gpCircle2.AddEllipse(rectCircle2);
            Region region1 = new Region(gpCircle1); // Регион объединения окружностей
            region1.Union(gpCircle2);

            GraphicsPath triangle1 = new GraphicsPath(); // Треугольник СВЕРХУ
            Point[] points1 = { new Point(origin.X, origin.Y), new Point(origin.X + 2 * scale, origin.Y - 2 * scale), new Point(origin.X + 2 * scale, origin.Y), new Point(origin.X, origin.Y) };
            triangle1.AddPolygon(points1);
            gr.DrawLines(new Pen(Color.DarkRed, 1), points1);
            gr.DrawLine(new Pen(Color.Black, 2), origin.X - 2 * scale, origin.Y + 2 * scale, origin.X + 3 * scale, origin.Y - 3 * scale);
            Region RegResult1 = new Region(triangle1);  // Финальный регион СНИЗУ
            RegResult1.Intersect(gpCircle2);

            gr.FillRegion(br3, RegResult1);

            GraphicsPath triangle2 = new GraphicsPath(); // Треугольник СНИЗУ
            Point[] points2 = { new Point(origin.X, origin.Y), new Point(origin.X, origin.Y - 2 * scale), new Point(origin.X + 2 * scale, origin.Y - 2 * scale), new Point(origin.X, origin.Y) };
            triangle2.AddPolygon(points2);
            gr.DrawLines(new Pen(Color.DarkRed, 1), points2);
            Region RegResult2 = new Region(gpCircle1); // Финальный регион СВЕРХУ
            RegResult2.Intersect(triangle2);
            RegResult2.Exclude(gpCircle2);

            gr.FillRegion(br4, RegResult2);

            reg1 = RegResult1;
            reg2 = RegResult2;

        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            int curX = e.X, curY = e.Y;

            bool clickX = (curX >= recX.X && curX <= recX.X + recX.Width && curY >= recX.Y && curY <= recX.Y + recY.Height);
            bool clickY = (curX >= recY.X && curX <= recY.X + recX.Width && curY >= recY.Y && curY <= recY.Y + recY.Height);

            if (clickX)
            {
                DropStarted = true;
                activeElem = 0;
            }
            else if (clickY)
            {
                DropStarted = true;
                activeElem = 1;
            }
            else
                DropStarted = false;
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (DropStarted)
            {
                if (activeElem == 0)
                {
                    recX.X = e.X - recX.Width / 2;
                    recX.Y = e.Y - recX.Height / 2;
                }
                else if (activeElem == 1)
                {
                    recY.X = e.X - recY.Width / 2;
                    recY.Y = e.Y - recY.Height / 2;
                }
                Invalidate();
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            SolidBrush newBr1 = new SolidBrush(br3.Color);
            SolidBrush newBr2 = new SolidBrush(br4.Color);

            if (reg1.IsVisible(e.X, e.Y))
                newBr1.Color = hoverBrush.Color;
            else
                newBr1.Color = defaultBrush.Color;

            if (reg2.IsVisible(e.X, e.Y))
                newBr2.Color = hoverBrush.Color;
            else
                newBr2.Color = defaultBrush.Color;

            if (newBr1.Color != br3.Color)
            {
                br3 = newBr1;
                Invalidate();
            }

            if (newBr2.Color != br4.Color)
            {
                br4 = newBr2;
                Invalidate();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                hoverBrush.Color = dlg.Color;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!stop)
            {
                if (rgb > 250)
                    growing = false;
                if (rgb < 10)
                    growing = true;

                if (growing)
                    rgb += 2;
                else
                    rgb -= 2;

                this.button1.BackColor = Color.FromArgb(rgb, rgb, 200, 222);
                this.button2.BackColor = Color.FromArgb(rgb, rgb, 200, 222);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            stop = !stop;
        }
    }
}
