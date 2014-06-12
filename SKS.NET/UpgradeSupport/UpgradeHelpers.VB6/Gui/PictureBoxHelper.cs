using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace UpgradeHelpers.VB6.Gui
{
    /// <summary>
    /// Helper to support painting in controls/forms.
    /// </summary>
    public class PictureBoxHelper
    {
        /// <summary>
        /// Paints an Image in the specified position and size.
        /// </summary>
        /// <param name="mpicture">The control where to paint.</param>
        /// <param name="Picture">The image to paint.</param>
        /// <param name="X1">The position in the X axis.</param>
        /// <param name="Y1">The position in the Y axis.</param>
        public static void PaintPicture(PictureBox mpicture, object Picture, double X1, double Y1)
        {
            if (Picture is System.Drawing.Image)
            {
                System.Drawing.Image img = (System.Drawing.Image)Picture;
                PaintPicture(mpicture, Picture, X1, Y1, Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(img.Width), Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(img.Height));
            }
            else
            {
                //TODO: ToBeImplemented
                throw new System.Exception("Method or Property not implemented yet!");
            }
        }

        /// <summary>
        /// Paints an Image in the specified position and size.
        /// </summary>
        /// <param name="mpicture">The control where to paint.</param>
        /// <param name="Picture">The image to paint.</param>
        /// <param name="X1">The position in the X axis.</param>
        /// <param name="Y1">The position in the Y axis.</param>
        /// <param name="Width1">The width used to paint the image.</param>
        public static void PaintPicture(PictureBox mpicture, object Picture, double X1, double Y1, double Width1)
        {
            if (Picture is System.Drawing.Image)
            {
                System.Drawing.Image img = (System.Drawing.Image)Picture;
                PaintPicture(mpicture, Picture, X1, Y1, Width1, Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(img.Height));
            }
            else
            {
                //TODO: ToBeImplemented
                throw new System.Exception("Method or Property not implemented yet!");
            }
        }

        /// <summary>
        /// Paints an Image in the specified position and size.
        /// </summary>
        /// <param name="mpicture">The control where to paint.</param>
        /// <param name="Picture">The image to paint.</param>
        /// <param name="X1">The position in the X axis.</param>
        /// <param name="Y1">The position in the Y axis.</param>
        /// <param name="Width1">The width used to paint the image.</param>
        /// <param name="Height1">The height used to paint the image.</param>
        public static void PaintPicture(PictureBox mpicture, object Picture, double X1, double Y1, double Width1, double Height1)
        {
            Rectangle rec = new Rectangle((int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX(X1), (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY(Y1), (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX(Width1), (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY(Height1));

            if (Picture is System.Drawing.Image)
            {
                System.Drawing.Image img = (System.Drawing.Image)Picture;
                mpicture.CreateGraphics().DrawImage(img, rec);

                //mpicture.CreateGraphics().drawi
            }
            else
            {
                //TODO: ToBeImplemented
                throw new System.Exception("Method or Property not implemented yet!");
            }
        }

        /// <summary>
        /// Paints an Image in the specified position and size.
        /// </summary>
        /// <param name="mpicture">The control where to paint.</param>
        /// <param name="Picture">The image to paint.</param>
        /// <param name="X1">The position in the X axis.</param>
        /// <param name="Y1">The position in the Y axis.</param>
        /// <param name="Width1">The width used to paint the image.</param>
        /// <param name="Height1">The height used to paint the image.</param>
        /// <param name="X2">This argument is discarded.</param>
        public static void PaintPicture(PictureBox mpicture, object Picture, double X1, double Y1, double Width1, double Height1, double X2)
        {
            if (Picture is System.Drawing.Image)
            {
                PaintPicture(mpicture, Picture, X1, Y1, Width1, Height1);
            }
            else
            {
                //TODO: ToBeImplemented
                throw new System.Exception("Method or Property not implemented yet!");
            }
        }

        /// <summary>
        /// Paints an Image in the specified position and size.
        /// </summary>
        /// <param name="mpicture">The control where to paint.</param>
        /// <param name="Picture">The image to paint.</param>
        /// <param name="X1">The position in the X axis.</param>
        /// <param name="Y1">The position in the Y axis.</param>
        /// <param name="Width1">The width used to paint the image.</param>
        /// <param name="Height1">The height used to paint the image.</param>
        /// <param name="X2">This argument is discarded.</param>
        /// <param name="Y2">This argument is discarded.</param>
        public static void PaintPicture(PictureBox mpicture, object Picture, double X1, double Y1, double Width1, double Height1, double X2, double Y2)
        {
            if (Picture is System.Drawing.Image)
            {
                PaintPicture(mpicture, Picture, X1, Y1, Width1, Height1);
            }
            else
            {
                //TODO: ToBeImplemented
                throw new System.Exception("Method or Property not implemented yet!");
            }
        }

        /// <summary>
        /// Paints an Image in the specified position and size.
        /// </summary>
        /// <param name="mpicture">The control where to paint.</param>
        /// <param name="Picture">The image to paint.</param>
        /// <param name="X1">The position in the X axis.</param>
        /// <param name="Y1">The position in the Y axis.</param>
        /// <param name="Width1">The width used to paint the image.</param>
        /// <param name="Height1">The height used to paint the image.</param>
        /// <param name="X2">This argument is discarded.</param>
        /// <param name="Y2">This argument is discarded.</param>
        /// <param name="Width2">This argument is discarded.</param>
        public static void PaintPicture(PictureBox mpicture, object Picture, double X1, double Y1, double Width1, double Height1, double X2, double Y2, double Width2)
        {
            if (Picture is System.Drawing.Image)
            {
                PaintPicture(mpicture, Picture, X1, Y1, Width1, Height1);
            }
            else
            {
                //TODO: ToBeImplemented
                throw new System.Exception("Method or Property not implemented yet!");
            }
        }

        /// <summary>
        /// Paints an Image in the specified position and size.
        /// </summary>
        /// <param name="mpicture">The control where to paint.</param>
        /// <param name="Picture">The image to paint.</param>
        /// <param name="X1">The position in the X axis.</param>
        /// <param name="Y1">The position in the Y axis.</param>
        /// <param name="Width1">The width used to paint the image.</param>
        /// <param name="Height1">The height used to paint the image.</param>
        /// <param name="X2">This argument is discarded.</param>
        /// <param name="Y2">This argument is discarded.</param>
        /// <param name="Width2">This argument is discarded.</param>
        /// <param name="Height2">This argument is discarded.</param>
        public static void PaintPicture(PictureBox mpicture, object Picture, double X1, double Y1, double Width1, double Height1, double X2, double Y2, double Width2, double Height2)
        {
            if (Picture is System.Drawing.Image)
            {
                PaintPicture(mpicture, Picture, X1, Y1, Width1, Height1);
            }
            else
            {
                //TODO: ToBeImplemented
                throw new System.Exception("Method or Property not implemented yet!");
            }
        }

        /// <summary>
        /// Paints an Image in the specified position and size.
        /// </summary>
        /// <param name="mpicture">The control where to paint.</param>
        /// <param name="Picture">The image to paint.</param>
        /// <param name="X1">The position in the X axis.</param>
        /// <param name="Y1">The position in the Y axis.</param>
        /// <param name="Width1">The width used to paint the image.</param>
        /// <param name="Height1">The height used to paint the image.</param>
        /// <param name="X2">This argument is discarded.</param>
        /// <param name="Y2">This argument is discarded.</param>
        /// <param name="Width2">This argument is discarded.</param>
        /// <param name="Height2">This argument is discarded.</param>
        /// <param name="Opcode">This argument is discarded.</param>
        public static void PaintPicture(PictureBox mpicture, object Picture, double X1, double Y1, double Width1, double Height1, double X2, double Y2, double Width2, double Height2, int Opcode)
        {
            if (Picture is System.Drawing.Image)
            {
                PaintPicture(mpicture, Picture, X1, Y1, Width1, Height1);
            }
            else
            {
                //TODO: ToBeImplemented
                throw new System.Exception("Method or Property not implemented yet!");
            }
        }
    }
}
