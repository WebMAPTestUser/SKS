using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace UpgradeHelpers.VB6.Gui
{
    /// <summary>
    /// Helper to support VB6 Shape controls.
    /// </summary>
    public partial class ShapeHelper : UserControl, IDisposable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ShapeHelper()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.UserPaint, true);
            base.BackColor = System.Drawing.Color.Transparent;

        }

        /// <summary>
        /// Enums for the shapes values.
        /// </summary>
        private enum ShapesEnum
        {
            Rectangle = 0,
            Square = 1,
            Oval = 2,
            Circle = 3,
            RoundRectangle = 4,
            RoundSquare = 5
        }

        /// <summary>
        /// BackStyle enum.
        /// </summary>
        private enum BackStyleEnum
        {
            Transparent = 0,
            Opaque = 1
        }

        /// <summary>
        /// Enums for FillStyle.
        /// </summary>
        private enum FillStyleEnum
        {
            Solid = 0,
            Transparent = 1,
            HorizontalLine = 2,
            VerticalLine = 3,
            DownwardDiagonal = 4,
            UpwardDiagonal = 5,
            Cross = 6,
            DiagonalCross = 7
        }


        /// <summary>
        /// Stores the BackColor property.
        /// </summary>
        private System.Drawing.Color _BackColor = SystemColors.Control;
        /// <summary>
        /// Brush used to paint the Shape control.
        /// </summary>
        private System.Drawing.Drawing2D.HatchBrush sBrush =
            new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.Horizontal, Color.Transparent, Color.Transparent);

        /// <summary>
        /// Backgorund Color to display text and graphics.
        /// </summary>
        [Description("Returns/sets the background color used to display text and graphics in an object."), Category("Appearance")]
        public new System.Drawing.Color BackColor
        {
            get
            {
                return _BackColor;
            }
            set
            {
                _BackColor = value;
                this.Refresh();
            }
        }

        /// <summary>
        /// Stores the BackStyle property.
        /// </summary>
        private BackStyleEnum _BackStyle = BackStyleEnum.Transparent;
        /// <summary>
        /// Indicates whether a Label or the background of a Shape is transparent or opaque.
        /// </summary>
        [Description("Indicates whether a Label or the background of a Shape is transparent or opaque."), Category("Appearance")]
        public int BackStyle
        {
            get
            {
                return (int)_BackStyle;
            }
            set
            {
                _BackStyle = (value == 0 ? BackStyleEnum.Transparent : BackStyleEnum.Opaque);
                if ((_BackStyle == BackStyleEnum.Opaque) && (_FillStyle == FillStyleEnum.Transparent))
                {
                    sBrush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.Horizontal, BackColor, BackColor);
                }

                this.Refresh();
            }
        }

        /// <summary>
        /// Pen used to paint the Shape Control.
        /// </summary>
        private Pen sPen = new Pen(SystemColors.WindowText);
        /// <summary>
        /// Stores the BorderColor property.
        /// </summary>
        private System.Drawing.Color _BorderColor = SystemColors.WindowText;
        /// <summary>
        /// Color of the Shape border.
        /// </summary>
        [Description("Returns/sets the color of an object's border."), Category("Appearance")]
        public System.Drawing.Color BorderColor
        {
            get
            {
                return _BorderColor;
            }
            set
            {
                _BorderColor = value;
                if (BorderStyle != 0)
                    sPen.Color = value;

                this.Refresh();
            }
        }

        /// <summary>
        /// Stores the BorderStyle property.
        /// </summary>
        private int _BorderStyle = 1;
        /// <summary>
        /// Border style of the Shape control.
        /// </summary>
        [Description("Returns/sets the border style for an object."), Category("Appearance")]
        public new int BorderStyle
        {
            get
            {
                return _BorderStyle;
            }
            set
            {
                sPen.DashOffset = 1500;
                switch (value)
                {
                    case 0:
                        _BorderStyle = 0;
                        sPen.Color = Color.Transparent;
                        break;
                    case 2:
                        _BorderStyle = 2;
                        sPen.Color = BorderColor;
                        sPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        break;
                    case 3:
                        _BorderStyle = 3;
                        sPen.Color = BorderColor;
                        sPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                        break;
                    case 4:
                        _BorderStyle = 4;
                        sPen.Color = BorderColor;
                        sPen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;
                        break;
                    case 5:
                        _BorderStyle = 5;
                        sPen.Color = BorderColor;
                        sPen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;
                        break;
                    case 6:
                        _BorderStyle = 6;
                        sPen.Color = BorderColor;
                        sPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                        break;
                    default:
                        _BorderStyle = 1;
                        sPen.Color = BorderColor;
                        sPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                        break;
                }

                this.Refresh();
            }
        }

        /// <summary>
        /// Stores the BorderWidth property.
        /// </summary>
        private int _BorderWidth = 1;
        /// <summary>
        /// Width of the Shape border.
        /// </summary>
        [Description("Returns or sets the width of a control's border."), Category("Appearance")]
        public int BorderWidth
        {
            get
            {
                return _BorderWidth;
            }
            set
            {
                _BorderWidth = value;
                sPen.Width = _BorderWidth;
                this.Refresh();
            }
        }

        /// <summary>
        /// Stores FillColor property.
        /// </summary>
        private System.Drawing.Color _FillColor = Color.Black;
        /// <summary>
        /// Color to fill in Shape control.
        /// </summary>
        [Description("Returns/sets the color used to fill in shapes, circles, and boxes"), Category("Appearance")]
        public System.Drawing.Color FillColor
        {
            get
            {
                return _FillColor;
            }
            set
            {
                _FillColor = value;
                FillStyle = FillStyle;
                this.Refresh();
            }
        }

        /// <summary>
        /// Stores FillStyle property.
        /// </summary>
        private FillStyleEnum _FillStyle = FillStyleEnum.Solid;
        /// <summary>
        /// FillStyle in Shape control.
        /// </summary>
        [Description("Returns/sets the fill style of a shape"), Category("Appearance")]
        public int FillStyle
        {
            get
            {
                return (int)_FillStyle;
            }
            set
            {
                switch (value)
                {
                    case 0:
                        _FillStyle = FillStyleEnum.Solid;
                        sBrush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.Horizontal, FillColor, FillColor);
                        break;
                    case 2:
                        _FillStyle = FillStyleEnum.HorizontalLine;
                        if (_BackStyle == BackStyleEnum.Opaque)
                            sBrush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.Horizontal, FillColor, BackColor);
                        else
                            sBrush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.Horizontal, FillColor, Color.Transparent);
                        break;
                    case 3:
                        _FillStyle = FillStyleEnum.VerticalLine;
                        if (_BackStyle == BackStyleEnum.Opaque)
                            sBrush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.Vertical, FillColor, BackColor);
                        else
                            sBrush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.Vertical, FillColor, Color.Transparent);
                        break;
                    case 4:
                        _FillStyle = FillStyleEnum.DownwardDiagonal;
                        if (_BackStyle == BackStyleEnum.Opaque)
                            sBrush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.WideDownwardDiagonal, FillColor, BackColor);
                        else
                            sBrush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.WideDownwardDiagonal, FillColor, Color.Transparent);
                        break;
                    case 5:
                        _FillStyle = FillStyleEnum.UpwardDiagonal;
                        if (_BackStyle == BackStyleEnum.Opaque)
                            sBrush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.WideUpwardDiagonal, FillColor, BackColor);
                        else
                            sBrush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.WideUpwardDiagonal, FillColor, Color.Transparent);
                        break;
                    case 6:
                        _FillStyle = FillStyleEnum.Cross;
                        if (_BackStyle == BackStyleEnum.Opaque)
                            sBrush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.Cross, FillColor, BackColor);
                        else
                            sBrush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.Cross, FillColor, Color.Transparent);
                        break;
                    case 7:
                        _FillStyle = FillStyleEnum.DiagonalCross;
                        if (_BackStyle == BackStyleEnum.Opaque)
                            sBrush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.DiagonalCross, FillColor, BackColor);
                        else
                            sBrush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.DiagonalCross, FillColor, Color.Transparent);
                        break;
                    default:
                        _FillStyle = FillStyleEnum.Transparent;
                        if (_BackStyle == BackStyleEnum.Transparent)
                            sBrush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.Horizontal, Color.Transparent, Color.Transparent);
                        else
                            sBrush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.Horizontal, BackColor, BackColor);
                        break;
                }
                this.Refresh();
            }
        }

        /// <summary>
        /// Stores the Shape property.
        /// </summary>
        private ShapesEnum _Shape = ShapesEnum.Rectangle;
        /// <summary>
        /// The kind of Shape.
        /// </summary>
        [Description("Returns/sets a value indicating the appearance of a control"), Category("Appearance")]
        public int Shape
        {
            get
            {
                return (int)_Shape;
            }
            set
            {
                switch (value)
                {
                    case 1:
                        _Shape = ShapesEnum.Square;
                        break;
                    case 2:
                        _Shape = ShapesEnum.Oval;
                        break;
                    case 3:
                        _Shape = ShapesEnum.Circle;
                        break;
                    case 4:
                        _Shape = ShapesEnum.RoundRectangle;
                        break;
                    case 5:
                        _Shape = ShapesEnum.RoundSquare;
                        break;
                    default:
                        _Shape = ShapesEnum.Rectangle;
                        break;
                }
                this.Refresh();
            }
        }

        /// <summary>
        /// Stores the RoundPercent property.
        /// </summary>
        private double _RoundPercent = 0.15;
        /// <summary>
        /// Adds a property to specify the percent used to 
        /// round the corners in round rectangles and round squares.
        /// </summary>
        [Description("Allows to specify the percent used to round the corners of round rectangles and round squares")]
        public int RoundPercent
        {
            get { return (int)(_RoundPercent * 100); }
            set
            {
                if ((value < 1) || (value > 50))
                    throw new InvalidConstraintException("Invalid property value");
                else
                {
                    _RoundPercent = (double)value / 100;
                    this.Refresh();
                }
            }
        }

        /// <summary>
        /// Manages the paint event of the Shape control.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The Paint event arguments.</param>
        private void ShapeHelper_Paint(object sender, PaintEventArgs e)
        {
            Rectangle clientRectangle = new Rectangle(1, 1, this.ClientRectangle.Width - 2, this.ClientRectangle.Height - 2);
            switch (Shape)
            {
                case (int)ShapesEnum.Rectangle:
                    DrawRectangle(clientRectangle, e.Graphics);
                    break;
                case (int)ShapesEnum.Square:
                    DrawSquare(clientRectangle, e.Graphics);
                    break;
                case (int)ShapesEnum.Oval:
                    DrawOval(clientRectangle, e.Graphics);
                    break;
                case (int)ShapesEnum.Circle:
                    DrawCircle(clientRectangle, e.Graphics);
                    break;
                case (int)ShapesEnum.RoundRectangle:
                    DrawRoundRectangle(clientRectangle, e.Graphics);
                    break;
                case (int)ShapesEnum.RoundSquare:
                    DrawRoundSquare(clientRectangle, e.Graphics);
                    break;
            }
        }

        /// <summary>
        /// Manages the Resize event to force the repaint.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ShapeHelper_Resize(object sender, EventArgs e)
        {
            this.Refresh();
        }

        /// <summary>
        /// Draws a round square.
        /// </summary>
        /// <param name="clientRectangle"></param>
        /// <param name="g"></param>
        private void DrawRoundSquare(Rectangle clientRectangle, Graphics g)
        {
            int MaxDiameter = Math.Min(clientRectangle.Height, clientRectangle.Width);
            Rectangle newClientRectangle = new Rectangle(clientRectangle.Location.X + (clientRectangle.Width - MaxDiameter) / 2,
                clientRectangle.Location.Y + (clientRectangle.Height - MaxDiameter) / 2, MaxDiameter, MaxDiameter);

            DrawRoundRectangle(newClientRectangle, g);
        }

        /// <summary>
        /// Draws a round rectangle.
        /// </summary>
        /// <param name="clientRectangle">The region where to draw.</param>
        /// <param name="g">The GDI used to draw the rectangle.</param>
        private void DrawRoundRectangle(Rectangle clientRectangle, Graphics g)
        {
            double percentX, percentY, halfPercentX, halfPercentY, minPercent, minHalfPercent;
            percentX = clientRectangle.Width * _RoundPercent;
            percentY = clientRectangle.Height * _RoundPercent;
            minPercent = Math.Min(percentX, percentY);
            halfPercentX = percentX / 2;
            halfPercentY = percentY / 2;
            minHalfPercent = Math.Min(halfPercentX, halfPercentY);

            PointF pUp1 = new PointF((float)(clientRectangle.X + minPercent), clientRectangle.Y);
            PointF pUp2 = new PointF((float)(clientRectangle.X + clientRectangle.Width - minPercent), clientRectangle.Y);

            PointF pDown1 = new PointF((float)(clientRectangle.X + clientRectangle.Width - minPercent), clientRectangle.Y + clientRectangle.Height);
            PointF pDown2 = new PointF((float)(clientRectangle.X + minPercent), clientRectangle.Y + clientRectangle.Height);

            PointF pLeft1 = new PointF(clientRectangle.X, (float)(clientRectangle.Y + clientRectangle.Height - minPercent));
            PointF pLeft2 = new PointF(clientRectangle.X, (float)(clientRectangle.Y + minPercent));

            PointF pRight1 = new PointF(clientRectangle.X + clientRectangle.Width, (float)(clientRectangle.Y + minPercent));
            PointF pRight2 = new PointF(clientRectangle.X + clientRectangle.Width, (float)(clientRectangle.Y + clientRectangle.Height - minPercent));



            PointF pCornerA1 = new PointF(clientRectangle.X, (float)(clientRectangle.Y + minHalfPercent));
            PointF pCornerA2 = new PointF((float)(clientRectangle.X + minHalfPercent), clientRectangle.Y);

            PointF pCornerB1 = new PointF((float)(clientRectangle.X + clientRectangle.Width - minHalfPercent), clientRectangle.Y);
            PointF pCornerB2 = new PointF(clientRectangle.X + clientRectangle.Width, (float)(clientRectangle.Y + minHalfPercent));

            PointF pCornerC1 = new PointF(clientRectangle.X + clientRectangle.Width, (float)(clientRectangle.Y + clientRectangle.Height - minHalfPercent));
            PointF pCornerC2 = new PointF((float)(clientRectangle.X + clientRectangle.Width - minHalfPercent), clientRectangle.Y + clientRectangle.Height);

            PointF pCornerD1 = new PointF((float)(clientRectangle.X + minHalfPercent), clientRectangle.Y + clientRectangle.Height);
            PointF pCornerD2 = new PointF(clientRectangle.X, (float)(clientRectangle.Y + clientRectangle.Height - minHalfPercent));

            if ((_BackStyle != BackStyleEnum.Transparent) || (_FillStyle != FillStyleEnum.Transparent))
            {
                using (GraphicsPath gPath = new GraphicsPath())
                {
                    gPath.AddLine(pUp1, pUp2);
                    gPath.AddBezier(pUp2, pCornerB1, pCornerB2, pRight1);
                    gPath.AddLine(pRight1, pRight2);
                    gPath.AddBezier(pRight2, pCornerC1, pCornerC2, pDown1);
                    gPath.AddLine(pDown1, pDown2);
                    gPath.AddBezier(pDown2, pCornerD1, pCornerD2, pLeft1);
                    gPath.AddLine(pLeft1, pLeft2);
                    gPath.AddBezier(pLeft2, pCornerA1, pCornerA2, pUp1);
                    using (Region region = new Region(gPath))
                    {
                        g.FillRegion(sBrush, region);
                    }
                }
            }

            g.DrawLine(sPen, pUp1, pUp2);
            g.DrawLine(sPen, pDown1, pDown2);
            g.DrawLine(sPen, pLeft1, pLeft2);
            g.DrawLine(sPen, pRight1, pRight2);

            g.DrawBezier(sPen, pLeft2, pCornerA1, pCornerA2, pUp1);
            g.DrawBezier(sPen, pUp2, pCornerB1, pCornerB2, pRight1);
            g.DrawBezier(sPen, pRight2, pCornerC1, pCornerC2, pDown1);
            g.DrawBezier(sPen, pDown2, pCornerD1, pCornerD2, pLeft1);
        }

        /// <summary>
        /// Draws a circle.
        /// </summary>
        /// <param name="clientRectangle">The region where to draw.</param>
        /// <param name="g">The GDI used to draw the rectangle.</param>
        private void DrawCircle(Rectangle clientRectangle, Graphics g)
        {
            int MaxDiameter = Math.Min(clientRectangle.Height, clientRectangle.Width);
            Rectangle newClientRectangle = new Rectangle(clientRectangle.Location.X + (clientRectangle.Width - MaxDiameter) / 2,
                clientRectangle.Location.Y + (clientRectangle.Height - MaxDiameter) / 2, MaxDiameter, MaxDiameter);

            if ((_BackStyle != BackStyleEnum.Transparent) || (_FillStyle != FillStyleEnum.Transparent))
                g.FillEllipse(sBrush, newClientRectangle);

            g.DrawEllipse(sPen, newClientRectangle);
        }

        /// <summary>
        /// Draws an oval.
        /// </summary>
        /// <param name="clientRectangle">The region where to draw.</param>
        /// <param name="g">The GDI used to draw the rectangle.</param>
        private void DrawOval(Rectangle clientRectangle, Graphics g)
        {
            if ((_BackStyle != BackStyleEnum.Transparent) || (_FillStyle != FillStyleEnum.Transparent))
                g.FillEllipse(sBrush, clientRectangle);

            g.DrawEllipse(sPen, clientRectangle);
        }

        /// <summary>
        /// Draws a square.
        /// </summary>
        /// <param name="clientRectangle">The region where to draw.</param>
        /// <param name="g">The GDI used to draw the rectangle.</param>
        private void DrawSquare(Rectangle clientRectangle, Graphics g)
        {
            int MaxDiameter = Math.Min(clientRectangle.Height, clientRectangle.Width);
            Rectangle newClientRectangle = new Rectangle(clientRectangle.Location.X + (clientRectangle.Width - MaxDiameter) / 2,
                clientRectangle.Location.Y + (clientRectangle.Height - MaxDiameter) / 2, MaxDiameter, MaxDiameter);

            if ((_BackStyle != BackStyleEnum.Transparent) || (_FillStyle != FillStyleEnum.Transparent))
                g.FillRectangle(sBrush, newClientRectangle);

            g.DrawRectangle(sPen, newClientRectangle);
        }

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="clientRectangle">The region where to draw.</param>
        /// <param name="g">The GDI used to draw the rectangle.</param>
        private void DrawRectangle(Rectangle clientRectangle, Graphics g)
        {
            if ((_BackStyle != BackStyleEnum.Transparent) || (_FillStyle != FillStyleEnum.Transparent))
                g.FillRectangle(sBrush, clientRectangle);

            g.DrawRectangle(sPen, clientRectangle);
        }

        /// <summary>
        /// Overrinding CreateParams method from UserControl.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; //WS_EX_TRANSPARENT 
                return cp;
            }
        }

        /// <summary>
        /// Overriding OnPaintBackground method from UserControl.
        /// </summary>
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //do not allow the background to be painted  
        }
    }
}
