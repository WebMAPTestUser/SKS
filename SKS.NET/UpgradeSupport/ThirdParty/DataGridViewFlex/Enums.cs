using System;
using System.Collections.Generic;
using System.Text;

namespace UpgradeHelpers.Windows.Forms
{
    #region Enums

    /// <summary>
    /// Sort Constants
    /// </summary>
    public enum SortSettings
    {
        /// <summary>
        /// Generic ascending sort
        /// </summary>
        SortGenericAscending = 1,
        /// <summary>
        /// Generic descending sort
        /// </summary>
        SortGenericDescending = 2,
        /// <summary>
        /// No Sort
        /// </summary>
        SortNone = 0,
        /// <summary>
        /// Numeric ascending sort
        /// </summary>
        SortNumericAscending = 3,
        /// <summary>
        /// Numeric descending sort
        /// </summary>
        SortNumericDescending = 4,
        /// <summary>
        ///  String ascending sort, case-sensitive
        /// </summary>
        SortStringAscending = 7,
        /// <summary>
        /// String descending sort, case-sensitive
        /// </summary>
        SortStringDescending = 8,
        /// <summary>
        /// String ascending sort, case-insensitive
        /// </summary>
        SortStringNoCaseAscending = 5,
        /// <summary>
        /// String descending sort, case-insensitive
        /// </summary>
        SortStringNoCaseDescending = 6
    }

    /// <summary>
    /// FillStyle Constants
    /// </summary>
    public enum FillStyleSettings
    {
        /// <summary>
        /// The Style Changes applies only to the Current Cell
        /// </summary>
        FillSingle = 0,
        /// <summary>
        /// The Style Changes applies to the Selected Cells
        /// </summary>
        FillRepeat = 1
    }

    /// <summary>
    /// AllowUserResizing Constants
    /// </summary>
    public enum AllowUserResizingSettings
    {
        /// <summary>
        /// None of the Columns or Rows could be Resized.
        /// </summary>
        ResizeNone = 0,
        /// <summary>
        /// Just Columns could be resized
        /// </summary>
        ResizeColumns = 1,
        /// <summary>
        /// Just Rows could be resized
        /// </summary>
        ResizeRows = 2,
        /// <summary>
        /// Both Rows and Columns be resized
        /// </summary>
        ResizeBoth = 3
    }



    /// <summary>
    /// Indicates the type of the Focus drawn in the control
    /// </summary>
    public enum FocusRectSettings
    {
        /// <summary>
        /// No Focus
        /// </summary>
        FocusNone = 0,
        /// <summary>
        /// Focus rect drawn lightly
        /// </summary>
        FocusLight = 1,
        /// <summary>
        /// Focus rect drawn more heavy
        /// </summary>
        FocusHeavy = 2
    }

    /// <summary>
    /// Indicates the type of Line used in the control
    /// </summary>
    public enum GridLineSettings
    {
        /// <summary>
        /// No Grid Line
        /// </summary>
        GridNone = 0,
        /// <summary>
        /// Plain type
        /// </summary>
        GridFlat = 1,
        /// <summary>
        /// Intern lines
        /// </summary>
        GridInset = 2,
        /// <summary>
        /// Extern Lines
        /// </summary>
        GridRaised = 3
    }

    /// <summary>
    /// Indicates highLight Type
    /// </summary>
    public enum HighLightSettings
    {
        /// <summary>
        /// Never Highlights
        /// </summary>
        HighlightNever = 0,
        /// <summary>
        /// Highlights always
        /// </summary>
        HighlightAlways = 1,
        /// <summary>
        /// Highlight With Focus
        /// </summary>
        HighlightWithFocus = 2
    }


    /// <summary>
    /// ScrollBar Constants
    /// </summary>
    public enum ScrollBarStyle
    {
        /// <summary>
        /// Neither Horizontal or Vertical ScrollBar
        /// </summary>
        ScrollBarNone = 0,
        /// <summary>
        /// Only Horizontal ScrollBar
        /// </summary>
        ScrollBarHorizontal = 1,
        /// <summary>
        /// Only Vertical ScrollBar
        /// </summary>
        ScrollBarVertical = 2,
        /// <summary>
        /// Both, Horizontal and Vertical ScrollBar
        /// </summary>
        ScrollBarBoth = 3
    }

    /// <summary>
    /// Controls the Style of the text
    /// </summary>
    public enum TextStyleSettings
    {
        /// <summary>
        /// Flat Text
        /// </summary>
        TextFlat = 0,
        /// <summary>
        /// Raised Text
        /// </summary>
        TextRaised = 1,
        /// <summary>
        /// Inset Text
        /// </summary>
        TextInset = 2,
        /// <summary>
        /// Raised Light
        /// </summary>
        TextRaisedLight = 3,
        /// <summary>
        /// Inset Light
        /// </summary>
        TextInsetLight = 4
    }

    #endregion

}
