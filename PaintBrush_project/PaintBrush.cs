﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaintBrush_project
{
    public partial class PaintBrush : Form
    {
        Bitmap bm;
        Graphics g;
        bool paint = false;
        Point px, py;
        Pen p = new Pen(Color.Black, 1);
        Pen erase = new Pen(Color.White);
        int x, y, sX, sY, cX, cY;
        ColorDialog cd =new ColorDialog();
        Color new_color;

        int index;
        public PaintBrush()
        {
            InitializeComponent();

            this.Width = 950;
            this.Height = 700;
            bm = new Bitmap(pic.Width, pic.Height);
            g= Graphics.FromImage(bm);
            g.Clear(Color.White);
            pic.Image = bm;


        }

        private void button1_Click(object sender, EventArgs e)
        {
            index = 3;
         }

        private void btnRectangle_Click(object sender, EventArgs e)
        {
            index = 4;
        }

        private void btnLine_Click(object sender, EventArgs e)
        {
            index = 5;
        }

        private void pic_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (paint)
            {
                if (index == 3)
                {
                    g.DrawEllipse(p, cX, cY, sX, sY);
                }
                if (index == 4)
                {
                    g.DrawRectangle(p, cX, cY, sX, sY);
                }
                if (index == 5)
                {
                    g.DrawLine(p, cX, cY, x, y);
                }
            }

        }

        private void pic_Click(object sender, EventArgs e)
        {

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            pic.Image = bm;
            index = 0;
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            cd.ShowDialog();
            new_color = cd.Color;
            btnpic_color.BackColor = cd.Color;
            p.Color = cd.Color;
        }

        private void pic_MouseDown(object sender, MouseEventArgs e)
        {
            paint = true;
            py= e.Location;
            cX = e.X;
            cY = e.Y;


        }

        private void pbColorPicker_MouseClick(object sender, MouseEventArgs e)
        {
            Point point = set_point(pbColorPicker, e.Location);
            btnpic_color.BackColor = ((Bitmap)pbColorPicker.Image).GetPixel(point.X, point.Y);
            new_color = btnpic_color.BackColor;
            p.Color = btnpic_color.BackColor;


        }

        private void pic_MouseUp(object sender, MouseEventArgs e)
        {
            paint = false;

            sX = x - cX;
            sY = y - cY;

            if(index == 3)
            {
                g.DrawEllipse(p, cX, cY, sX, sY);
            }
            if(index == 4)
            {
                g.DrawRectangle(p, cX, cY, sX, sY);
            }
            if(index == 5)
            {
                g.DrawLine(p,cX,cY,x,y);
            }
        }

        private void pic_MouseMove(object sender, MouseEventArgs e)
        {
            if (paint)
            {
                if(index == 1)
                {
                    px = e.Location;
                    g.DrawLine(p, px, py);
                    py = px;
                }
                if (index == 2)
                {
                    px = e.Location;
                    g.DrawLine(erase, px, py);
                    py = px;
                }
            }
            pic.Refresh();
            x = e.X;
            y = e.Y;
            sX = e.X - cX;
            sY = e.Y - cY;
        }

        private void pic_MouseClick(object sender, MouseEventArgs e)
        {
            if(index == 7)
            {
                Point point = set_point(pic, e.Location);
                Fill(bm, point.X, point.Y, new_color);
            }
        }

        private void btnFill_Click(object sender, EventArgs e)
        {
            index = 7;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "image(*.jpg)|*.jpg|(*.*|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Bitmap btm = bm.Clone(new Rectangle(0, 0, pic.Width, pic.Height), bm.PixelFormat);
                btm.Save(sfd.FileName, ImageFormat.Jpeg);
            }
        }

        private void btnPencil_Click(object sender, EventArgs e)
        {
            index = 1; 
        }

        private void btnEraser_Click(object sender, EventArgs e)
        {
            index = 2;
        }

        private void PaintBrush_Load(object sender, EventArgs e)
        {

        }
        static Point set_point(PictureBox pb,Point pt)
        {
            float pX = 1f *pb.Image.Width / pb.Width;
            float pY = 1f *pb.Image.Height / pb.Height;
            return new Point((int)(pt.X*pX),(int)(pt.Y*pY));

        }
        private void validate(Bitmap bm,Stack<Point>sp,int x,int y, Color old_color,Color new_color)
        {
            Color cx = bm.GetPixel(x, y);
            if(cx== old_color)
            {
                sp.Push(new Point(x,y));
                bm.SetPixel(x, y, new_color);
            }
        }
        public void Fill(Bitmap bm,int x, int y,Color new_clr)
        {
            Color old_color = bm.GetPixel(x, y);
            Stack<Point> pixel = new Stack<Point>();
            pixel.Push(new Point(x,y));
            bm.SetPixel(x,y,new_clr);
            if (old_color == new_clr) return;
            while(pixel.Count > 0)
            {
               Point pt= (Point)pixel.Pop();
                if (pt.X > 0 && pt.Y > 0 && pt.X < bm.Width - 1 && pt.Y < bm.Height - 1)
                {
                    validate(bm, pixel, pt.X-1, pt.Y, old_color, new_clr);
                    validate(bm, pixel, pt.X, pt.Y-1, old_color, new_clr);
                    validate(bm, pixel, pt.X+1, pt.Y, old_color, new_clr);
                    validate(bm, pixel, pt.X, pt.Y+1, old_color, new_clr);

                }
            }

        }
    }
}
