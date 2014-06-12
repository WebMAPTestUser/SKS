using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Reflection;
using StdFormat;
using System.Runtime.InteropServices.CustomMarshalers;

namespace UpgradeHelpers.VB6.Gui
{

    /// <summary>
    /// Imports the original MSDataGrid Splits interface.
    /// </summary>
    [ComImport, Guid("CDE57A53-8B86-11D0-B3C6-00A0C90AEA82"), TypeLibType((short)0x10c0)]
    public interface Splits : IEnumerable
    {
        /// <summary>
        /// Object Count
        /// </summary>
        [DispId(1)]
        int Count { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] get; }
        /// <summary>
        /// Get Enumerator
        /// </summary>
        /// <returns>IEnumerator</returns>
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "", MarshalTypeRef = typeof(EnumeratorToEnumVariantMarshaler), MarshalCookie = "")]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), TypeLibFunc((short)0x40), DispId(-4)]
        new IEnumerator GetEnumerator();
        /// <summary>
        /// Array Access
        /// </summary>
        /// <param name="Index">index to access</param>
        /// <returns></returns>
        [DispId(0)]
        Split this[object Index] { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0)] get; }
        /// <summary>
        /// Add object
        /// </summary>
        /// <param name="Index">index position to add to</param>
        /// <returns></returns>
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)]
        Split Add([In] short Index);
        /// <summary>
        /// Remove Object
        /// </summary>
        /// <param name="Index">index position to remove from</param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)]
        void Remove([In, MarshalAs(UnmanagedType.Struct)] object Index);
    }

    /// <summary>
    /// Imports the original MSDataGrid Split interface.
    /// </summary>
    [ComImport, TypeLibType((short)0x10c0), Guid("CDE57A54-8B86-11D0-B3C6-00A0C90AEA82")]
    public interface Split
    {
        /// <summary>
        /// Allow Focus
        /// </summary>
        [DispId(1)]
        bool AllowFocus { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] set; }
        /// <summary>
        /// Allow Row Sizing
        /// </summary>
        [DispId(2)]
        bool AllowRowSizing { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)] set; }
        /// <summary>
        /// Allow Sizing
        /// </summary>
        [DispId(3)]
        bool AllowSizing { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)] set; }
        /// <summary>
        /// Current Cell Visible
        /// </summary>
        [DispId(4)]
        bool CurrentCellVisible { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(4)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(4)] set; }
        /// <summary>
        /// First Row
        /// </summary>
        [DispId(5)]
        object FirstRow { [return: MarshalAs(UnmanagedType.Struct)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(5)] get; [param: In, MarshalAs(UnmanagedType.Struct)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(5)] set; }
        /// <summary>
        /// Actual Index
        /// </summary>
        [DispId(6)]
        short Index { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(6)] get; }
        /// <summary>
        /// Left Column
        /// </summary>
        [DispId(7)]        
        short LeftCol { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(7)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(7)] set; }
        /// <summary>
        /// Is Locked?
        /// </summary>
        [DispId(8)]
        bool Locked { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(8)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(8)] set; }
        /// <summary>
        /// Marquee Style
        /// </summary>
        [DispId(9)]
        MarqueeStyleConstants MarqueeStyle { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(9)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(9)] set; }
        /// <summary>
        /// Record Selectors
        /// </summary>
        [DispId(10)]
        bool RecordSelectors { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(10)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(10)] set; }
        /// <summary>
        /// ScrollBars
        /// </summary>
        [DispId(11)]
        ScrollBarsConstants ScrollBars { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(11)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(11)] set; }
        /// <summary>
        /// ScrollGroup
        /// </summary>
        [DispId(12)]
        short ScrollGroup { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(12)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(12)] set; }
        /// <summary>
        /// Select End Column
        /// </summary>
        [DispId(13)]
        short SelEndCol { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(13)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(13)] set; }
        /// <summary>
        /// Select Start Column
        /// </summary>
        [DispId(14)]
        short SelStartCol { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(14)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(14)] set; }
        /// <summary>
        /// Size object
        /// </summary>
        [DispId(15)]
        object Size { [return: MarshalAs(UnmanagedType.Struct)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(15)] get; [param: In, MarshalAs(UnmanagedType.Struct)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(15)] set; }
        /// <summary>
        /// Size Mode
        /// </summary>
        [DispId(0x10)]
        SplitSizeModeConstants SizeMode { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x10)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x10)] set; }
        /// <summary>
        /// Clear Selected Columns
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(100)]
        void ClearSelCols();
        /// <summary>
        /// Return Columns Object
        /// </summary>
        [DispId(0x65)]
        Columns Columns { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x65)] get; }
    }

    /// <summary>
    /// Imports the original MSDataGrid Columns interface.
    /// </summary>
    [ComImport, Guid("CDE57A50-8B86-11D0-B3C6-00A0C90AEA82"), TypeLibType((short)0x10c0)]
    public interface Columns : IEnumerable
    {
        /// <summary>
        /// Count
        /// </summary>
        [DispId(1)]
        int Count { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] get; }
        /// <summary>
        /// Get Enumerator
        /// </summary>
        /// <returns>IEnumerator</returns>
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "", MarshalTypeRef = typeof(EnumeratorToEnumVariantMarshaler), MarshalCookie = "")]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-4), TypeLibFunc((short)0x40)]
        new IEnumerator GetEnumerator();
        /// <summary>
        /// Add Column
        /// </summary>
        /// <param name="Index">at index</param>
        /// <returns>Column Object</returns>
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)]
        Column Add([In] short Index);
        /// <summary>
        /// Array Access
        /// </summary>
        /// <param name="Index">array position</param>
        /// <returns>Column Object</returns>
        [DispId(0)]
        Column this[object Index] { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0)] get; }
        /// <summary>
        /// Remove Column
        /// </summary>
        /// <param name="Index">at index</param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)]
        void Remove([In, MarshalAs(UnmanagedType.Struct)] object Index);
    }

    /// <summary>
    /// Imports the original MSDataGrid Column interface.
    /// </summary>
    [ComImport, Guid("CDE57A4F-8B86-11D0-B3C6-00A0C90AEA82"), DefaultMember("Text"), TypeLibType((short)0x10c0)]
    public interface Column
    {
        /// <summary>
        /// Alignment
        /// </summary>
        [DispId(1)]
        AlignmentConstants Alignment { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] set; }
        /// <summary>
        /// Allow Sizing
        /// </summary>
        [DispId(2)]
        bool AllowSizing { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)] set; }
        /// <summary>
        /// is Button?
        /// </summary>
        [DispId(3)]
        bool Button { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)] set; }
        /// <summary>
        /// Caption
        /// </summary>
        [DispId(-518)]
        string Caption { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-518)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-518)] set; }
        /// <summary>
        /// Column Index
        /// </summary>
        [DispId(4)]
        short ColIndex { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(4)] get; }
        /// <summary>
        /// Data changed
        /// </summary>
        [DispId(5)]
        bool DataChanged { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(5)] get; }
        /// <summary>
        /// DataField
        /// </summary>
        [DispId(6)]
        string DataField { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(6)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(6)] set; }
        /// <summary>
        /// DividerStyle
        /// </summary>
        [DispId(8)]
        DividerStyleConstants DividerStyle { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(8)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(8)] set; }
        /// <summary>
        /// Left
        /// </summary>
        [ComAliasName("stdole.OLE_XPOS_CONTAINER"), DispId(9)]
        float Left { [return: ComAliasName("stdole.OLE_XPOS_CONTAINER")] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(9)] get; }
        /// <summary>
        /// is Locked?
        /// </summary>
        [DispId(10)]
        bool Locked { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(10)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(10)] set; }
        /// <summary>
        /// Number Format
        /// </summary>
        [DispId(11)]
        string NumberFormat { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(11)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(11)] set; }
        /// <summary>
        /// Text
        /// </summary>
        [DispId(0)]
        string Text { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0)] set; }
        /// <summary>
        /// Top
        /// </summary>
        [DispId(12), ComAliasName("stdole.OLE_YPOS_CONTAINER")]
        float Top { [return: ComAliasName("stdole.OLE_YPOS_CONTAINER")] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(12)] get; }
        /// <summary>
        /// Value
        /// </summary>
        [DispId(13)]
        object Value { [return: MarshalAs(UnmanagedType.Struct)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(13)] get; [param: In, MarshalAs(UnmanagedType.Struct)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(13)] set; }
        /// <summary>
        /// Visible
        /// </summary>
        [DispId(14)]
        bool Visible { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(14)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(14)] set; }
        /// <summary>
        /// Width
        /// </summary>
        [DispId(15), ComAliasName("stdole.OLE_XSIZE_CONTAINER")]
        float Width { [return: ComAliasName("stdole.OLE_XSIZE_CONTAINER")] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(15)] get; [param: In, ComAliasName("stdole.OLE_XSIZE_CONTAINER")] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(15)] set; }
        /// <summary>
        /// WrapText
        /// </summary>
        [DispId(0x10)]
        bool WrapText { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x10)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x10)] set; }
        /// <summary>
        /// DataFormat
        /// </summary>
        [DispId(0x11)]
        IDataFormatDisp DataFormat { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x11)] get; [param: In, MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x11)] set; }
        /// <summary>
        /// CellText
        /// </summary>
        /// <param name="Bookmark">object</param>
        /// <returns>string value</returns>
        [return: MarshalAs(UnmanagedType.BStr)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(100)]
        string CellText([In, MarshalAs(UnmanagedType.Struct)] object Bookmark);
        /// <summary>
        /// CellValue
        /// </summary>
        /// <param name="Bookmark">object to Bookmark</param>
        /// <returns>cell object</returns>
        [return: MarshalAs(UnmanagedType.Struct)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x65)]
        object CellValue([In, MarshalAs(UnmanagedType.Struct)] object Bookmark);
    }

    /// <summary>
    /// Imports the original MSDataGrid AlignmentConstants enumeration.
    /// </summary>
    [Guid("CDE57A46-8B86-11D0-B3C6-00A0C90AEA82")]
    public enum AlignmentConstants
    {
        /// <summary>
        /// Left Alignment
        /// </summary>
        dbgLeft,
        /// <summary>
        /// Right Alignment
        /// </summary>
        dbgRight,
        /// <summary>
        /// Center Alignment
        /// </summary>
        dbgCenter,
        /// <summary>
        /// General Alignment
        /// </summary>
        dbgGeneral
    }

    /// <summary>
    /// Imports the original MSDataGrid DividerStyleConstants enumeration.
    /// </summary>
    [Guid("CDE57A4A-8B86-11D0-B3C6-00A0C90AEA82")]
    public enum DividerStyleConstants
    {
        /// <summary>
        /// No Divider
        /// </summary>
        dbgNoDividers,
        /// <summary>
        /// Black Line Divider
        /// </summary>
        dbgBlackLine,
        /// <summary>
        /// Gray Line Divider
        /// </summary>
        dbgDarkGrayLine,
        /// <summary>
        /// Raised Line Divider
        /// </summary>
        dbgRaised,
        /// <summary>
        /// Inset Line Divider
        /// </summary>
        dbgInset,
        /// <summary>
        /// Use ForeColor Line Divider
        /// </summary>
        dbgUseForeColor,
        /// <summary>
        /// LightGray Line Divider
        /// </summary>
        dbgLightGrayLine
    }

    /// <summary>
    /// Imports the original MSDataGrid MarqueeStyleConstants enumeration.
    /// </summary>
    [Guid("CDE57A4B-8B86-11D0-B3C6-00A0C90AEA82")]
    public enum MarqueeStyleConstants
    {
        /// <summary>
        /// Dotted Cell Border
        /// </summary>
        dbgDottedCellBorder,
        /// <summary>
        /// Solid border
        /// </summary>
        dbgSolidCellBorder,
        /// <summary>
        /// High light cell
        /// </summary>
        dbgHighlightCell,
        /// <summary>
        /// High Light Row
        /// </summary>
        dbgHighlightRow,
        /// <summary>
        /// High light Row Raised
        /// </summary>
        dbgHighlightRowRaiseCell,
        /// <summary>
        /// No marquee
        /// </summary>
        dbgNoMarquee,
        /// <summary>
        /// Floating editor
        /// </summary>
        dbgFloatingEditor
    }

    /// <summary>
    /// Imports the original MSDataGrid ScrollBarsConstants enumeration.
    /// </summary>
    [Guid("CDE57A4C-8B86-11D0-B3C6-00A0C90AEA82")]
    public enum ScrollBarsConstants
    {
        /// <summary>
        /// None
        /// </summary>
        dbgNone,
        /// <summary>
        /// Horizontal
        /// </summary>
        dbgHorizontal,
        /// <summary>
        /// Vertical
        /// </summary>
        dbgVertical,
        /// <summary>
        /// Vertical and Horizontal
        /// </summary>
        dbgBoth,
        /// <summary>
        /// Automatic
        /// </summary>
        dbgAutomatic
    }

    /// <summary>
    /// Imports the original MSDataGrid SplitSizeModeConstants enumeration.
    /// </summary>
    [Guid("CDE57A4D-8B86-11D0-B3C6-00A0C90AEA82")]
    public enum SplitSizeModeConstants
    {
        /// <summary>
        /// Scalable
        /// </summary>
        dbgScalable,
        /// <summary>
        /// Exact
        /// </summary>
        dbgExact
    }
}
