using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace UpgradeHelpers.VB6.Gui
{
	/// <summary>
	/// Control that shows a group of lines along the size of the control.
	/// </summary>
	public class BarsNet : System.Windows.Forms.Control
	{
		private Border3DStyle _lineBorderStyle;
		private int _spaceBetweenLines;

		/// <summary>
		/// Creates a new Bars control.
		/// </summary>
		public BarsNet()
		{
			this.AutoSize = false;
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.ResizeRedraw, true);
		}

		/// <summary>
		/// The lines style.
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
		/// Vertical separation space between the lines.
		/// </summary>
		[Description("The Style for the divider line."), Category("Appearance")]
		public int SpaceBetweenLines
		{
			get
			{
				if (_spaceBetweenLines == 0)
					_spaceBetweenLines = 4; 
				return _spaceBetweenLines;
			}
			set
			{
				if (value != _spaceBetweenLines)
				{
					_spaceBetweenLines = value;
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
		
			Point startingPoint = new Point( 5, 0);
			DrawLineAtGivenPoint(g, startingPoint);

			int linesToBeDrawn = this.Height - 5 / SpaceBetweenLines;
			for (int i = 0; i < linesToBeDrawn; i++)
			{
				startingPoint.Y += SpaceBetweenLines;
				DrawLineAtGivenPoint(g, startingPoint);
			}
		}

		private void DrawLineAtGivenPoint(Graphics graphContext, Point startingPoint)
		{
			ControlPaint.DrawBorder3D(graphContext, startingPoint.X,
							  startingPoint.Y,
							  this.Width - startingPoint.X,
							  5, LineBorderStyle, Border3DSide.Top);			
		}
	}
}
