using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace UpgradeHelpers.VB6.Gui
{
	/// <summary>
	/// Control that displays a label followed by a line until the end of the control size.
	/// </summary>
	public class LineHeader: System.Windows.Forms.Label
	{
		private int _spaceBetweenTextAndLine;
		private Border3DStyle _lineBorderStyle;

		/// <summary>
		/// Creates a new LineHeader control.
		/// </summary>
		public LineHeader()
		{
			this.AutoSize = false;
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.ResizeRedraw, true);
		}

		/// <summary>
		/// Separation between the label and the header line.
		/// </summary>
		[Description("The separation between the text and the divider line."),Category("Appearance")] 
		public int SpaceBetweenTextAndLine
		{ 
			get{return _spaceBetweenTextAndLine;}
			set
			{
				if(value != _spaceBetweenTextAndLine)
				{
					_spaceBetweenTextAndLine = value;
					this.Invalidate(); // Mark that the control require redraw.
				}
			} 
		}

		/// <summary>
		/// Style of the header line.
		/// </summary>
		[Description("The Style for the divider line."), Category("Appearance")] 
		public Border3DStyle LineBorderStyle
		{
			get{
				if (_lineBorderStyle == 0)
					_lineBorderStyle = Border3DStyle.Etched; // default style.
				return _lineBorderStyle; 
			}
			set{
				if (value != _lineBorderStyle)
				{
					_lineBorderStyle = value;
					this.Invalidate(); // Mark that the control require redraw.
				}
			}
		}
			
		/// <summary>
		/// Paints the control on the screen
		/// </summary>
		/// <param name="e">The context to paint</param>
		protected override void  OnPaint(PaintEventArgs e)
		{
			Graphics g  = e.Graphics;
			Font f = this.Font;
			Brush b  = new SolidBrush(this.ForeColor);
			StringFormat sf  = StringFormat.GenericTypographic;
			RectangleF labelBounds = new RectangleF(0, 0, this.Width, this.Height);
			SizeF textSize = g.MeasureString(this.Text, f, this.Width);
			g.DrawString(this.Text, f, b, 0, 0, sf);
			if(textSize.Width + SpaceBetweenTextAndLine < this.Width){
				Point startingPoint = new Point((int) textSize.Width + SpaceBetweenTextAndLine,
												(int) textSize.Height / 2);
				ControlPaint.DrawBorder3D(g, startingPoint.X,
										  startingPoint.Y,
										  this.Width - startingPoint.X,
										  5, LineBorderStyle, Border3DSide.Top);
			}		
		}
	}
}
