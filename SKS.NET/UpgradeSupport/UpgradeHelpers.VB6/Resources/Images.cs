using System;
using System.Collections.Generic;
using System.Text;
using stdole;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

namespace UpgradeHelpers.VB6.Resources
{
    /// <summary>
    /// The Images helper class provides several functions to handle pictures, icons and images.
    /// </summary>
    public class Images
    {
        /// <summary>
        /// CreateIconIndirect function from user32.dll.
        /// </summary>
        [DllImport("user32.dll")]
        private static extern IntPtr CreateIconIndirect(ref IconInfo icon);

        /// <summary>
        /// GetIconInfo function from user32.dll.
        /// </summary>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

        /// <summary>
        /// Structure to use to get info for a Icon.
        /// </summary>
        public struct IconInfo
        {
            /// <summary>
            /// use Icon
            /// </summary>
            public bool fIcon;
            /// <summary>
            /// x position
            /// </summary>
            public int xHotspot;
            /// <summary>
            /// y position
            /// </summary>
            public int yHotspot;
            /// <summary>
            /// Pointer to Mask
            /// </summary>
            public IntPtr hbmMask;
            /// <summary>
            /// Pointer to Palette
            /// </summary>
            public IntPtr hbmColor;
        }


        /// <summary>
        /// Converts a IPictureDisp to Icon.
        /// </summary>
        /// <param name="iPictureDisp">The picture to be converted.</param>
        /// <returns>The source picture as an Icon.</returns>
        public static Icon IPictureDispToIcon(IPictureDisp iPictureDisp)
        {
            return Icon.FromHandle(new IntPtr(iPictureDisp.Handle));
        }

        /// <summary>
        /// Converts an image into a cursor.
        /// </summary>
        /// <param name="source">The Image to be converted.</param>
        /// <returns>The source image as a Cursor.</returns>
        public static Cursor CreateCursor(Image source)
        {
            Bitmap bmpSource = source as Bitmap;
            if (bmpSource == null)
                bmpSource = new Bitmap(source);

            IconInfo iInfo = new IconInfo();
            GetIconInfo(bmpSource.GetHicon(), ref iInfo);
            iInfo.xHotspot = 3;
            iInfo.yHotspot = 3;
            iInfo.fIcon = false;

            return new Cursor(CreateIconIndirect(ref iInfo));
        }

        /// <summary>
        /// Converts a IPictureDisp into a cursor.
        /// </summary>
        /// <param name="source">The source IPicture to be converted.</param>
        /// <returns>The source IPicture as a Cursor.</returns>
        public static Cursor CreateCursor(IPictureDisp source)
        {
            Cursor res;
            Image sourceImg = null;
            Icon vb6Icon = null;
            try
            {
                sourceImg = Microsoft.VisualBasic.Compatibility.VB6.Support.IPictureToImage(source);
            }
            catch
            {
                try
                {
                    //In the case that the image is an Icon, this will convert it into a Bitmap
                    vb6Icon = IPictureDispToIcon(source);
                    sourceImg = vb6Icon.ToBitmap();
                }
                catch { }
            }

            res = CreateCursor(sourceImg);

            return res;
        }
    }
}
