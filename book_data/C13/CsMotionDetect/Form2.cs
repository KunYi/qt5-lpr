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
using System.Drawing;

namespace CsMotionDetect
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        Point mdp; //拖曳起點
        //開啟視窗，建立一個鏤空半透明外觀的視窗 
        private void Form2_Load(object sender, EventArgs e)
        {
            ClipWindow(); //鏤空視窗
        }
        //鏤空視窗
        private void ClipWindow()
        {
            GraphicsPath path = new GraphicsPath();
            Point[] pt = new Point[10];
            pt[0].X = 0; pt[0].Y = 0; //左上角
            pt[1].X = this.Width; pt[1].Y = 0; //右上角
            pt[2].X = this.Width; pt[2].Y = this.Height; //右下角
            pt[3].X = 0; pt[3].Y = this.Height; //左下角
            pt[4].X = 0; pt[4].Y = this.Height - 10;
            pt[5].X = this.Width - 10; pt[5].Y = this.Height - 10;
            pt[6].X = this.Width - 10; pt[6].Y = 10;
            pt[7].X = 10; pt[7].Y = 10;
            pt[8].X = 10; pt[8].Y = this.Height - 10;
            pt[9].X = 0; pt[9].Y = this.Height - 10;
            path.AddPolygon(pt); //以多邊形的方式加入path
            this.Region = new Region(path); //Region視窗區域
        }
        //拖曳視窗功能，定義視窗位置
        private void Form2_MouseDown(object sender, MouseEventArgs e)
        {
            mdp = e.Location;
        }

        private void Form2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.Location.X - mdp.X;
                this.Top += e.Location.Y - mdp.Y;
            }
        }

        //拖曳右下角功能，定義視窗寬高大小
        private void Label1_MouseDown(object sender, MouseEventArgs e)
        {
            mdp = e.Location;
        }

        private void Label1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                label1.Left += e.Location.X - mdp.X;
                label1.Top += e.Location.Y - mdp.Y;
                this.Width = label1.Right;
                this.Height = label1.Bottom;
                this.Refresh();
                ClipWindow();
            }
        }
    }
}
