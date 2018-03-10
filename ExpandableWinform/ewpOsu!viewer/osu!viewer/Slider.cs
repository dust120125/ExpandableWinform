using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace osu_viewer
{
    public partial class Slider : TrackBar
    {

        private Color barColor;
        private Color trackColor;
        private Color dotColor;
        public Color BaseBarColor
        {
            get { return barColor; }
            set
            {
                barColor = value;
                baseLine.Color = barColor;
                Update();
            }
        }

        public Color TrackBarColor
        {
            get { return trackColor; }
            set
            {
                trackColor = value;
                trackLine.Color = trackColor;
                Update();
            }
        }

        public Color PinColor
        {
            get { return dotColor; }
            set
            {
                dotColor = value;
                dotBrush = new SolidBrush(dotColor);
                Update();
            }
        }

        Pen baseLine;
        Pen trackLine;
        Brush backBrush;
        Brush dotBrush;

        int barWidth, dotSize, halfDot;
        int barPosX, barPosY;
        int currentValue;

        Graphics mainGraphic, buffGraphic;
        Bitmap doublebuffer;

        public Slider()
        {
            InitializeComponent();
            TickStyle = TickStyle.Both;
            SetStyle(ControlStyles.UserPaint, true);

            Maximum = 100;
            Value = 50;
            Height = 25;
            MinimumSize = new Size(60, 25);

            barWidth = Width - 20;
            barPosX = Width / 2 - barWidth / 2;
            barPosY = Height / 2;

            backBrush = new SolidBrush(BackColor);
            dotBrush = new SolidBrush(Color.CadetBlue);

            baseLine = new Pen(Color.Gray);
            baseLine.Width = 3;

            trackLine = new Pen(Color.CadetBlue);            
            trackLine.Width = 3;

            BaseBarColor = Color.Gray;
            TrackBarColor = Color.CadetBlue;
            PinColor = Color.CadetBlue;

            dotSize = 10;
            halfDot = dotSize / 2;

            doublebuffer = new Bitmap(Width, Height);
            buffGraphic = Graphics.FromImage(doublebuffer);
            mainGraphic = CreateGraphics();            
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            barWidth = Width - 20;
            barPosX = Width / 2 - barWidth / 2;
            barPosY = Height / 2;
            doublebuffer = new Bitmap(Width, Height);
            buffGraphic = Graphics.FromImage(doublebuffer);
            mainGraphic = CreateGraphics();
        }        

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            //mainGraphic.FillRectangle(backBrush, 0, 0, Width, Height);
            buffGraphic.FillRectangle(backBrush, ClientRectangle);

            int dotPosX = barWidth * Value / Maximum + barPosX;
            int dotPosY = this.Height / 2;
            
            buffGraphic.DrawLine(baseLine, barPosX, barPosY, barPosX + barWidth, barPosY);
            buffGraphic.DrawLine(trackLine, barPosX, barPosY, dotPosX, barPosY);
            buffGraphic.FillEllipse(dotBrush, dotPosX - halfDot, dotPosY - halfDot, dotSize, dotSize);
            if (Value != currentValue)
            {
                currentValue = Value;
                OnValueChanged(new EventArgs());
            }
            mainGraphic.DrawImage(doublebuffer, 0, 0);         
        }

        protected override void OnScroll(EventArgs e)
        {
            base.OnScroll(e);
        }

        protected override void OnValueChanged(EventArgs e)
        {
            base.OnValueChanged(e);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);
        }
    }
}
