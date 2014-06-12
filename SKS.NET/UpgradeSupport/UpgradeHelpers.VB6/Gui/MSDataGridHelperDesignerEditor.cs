using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Globalization;
using System.ComponentModel.Design.Serialization;
using System.Reflection;

namespace UpgradeHelpers.VB6.Gui
{
    /// <summary>
    /// Class to implement the custom editor of the property GridLayout.
    /// </summary>
    class MSDataGridHelperDesignerEditor : UITypeEditor
    {
        private IWindowsFormsEditorService editorService = null;

        /// <summary>
        /// Informs what will be the type of designer to be used, in this case it will be modal.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        /// <summary>
        /// Takes care of displaying the modal window to edit the current value.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="provider"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                editorService =
                    provider.GetService(
                    typeof(IWindowsFormsEditorService))
                    as IWindowsFormsEditorService;
            }

            if (editorService != null)
            {
                if (value != null)
                {
                    using (MSDataGridHelperDesignerModalView selectionControl = new MSDataGridHelperDesignerModalView((MSDataGridHelperLayoutInfo)value))
                    {
                        if (editorService.ShowDialog(selectionControl) == DialogResult.OK)
                            value = selectionControl.gridInfo;
                    }
                }
            }

            return value;
        }
    }

    /// <summary>
    /// Represents the grid layout for a MSDataGrid.  
    /// Stores and exposes the required properties to model the grid layout.
    /// </summary>
    [TypeConverter(typeof(MSDataGridHelperLayoutInfoConverter))]
    public class MSDataGridHelperLayoutInfo
    {
        private Splits _gridSplits = null;

        /// <summary>
        /// Obtains the GridSplits property.
        /// </summary>
        public Splits GridSplits
        {
            get
            {
                return _gridSplits;
            }
        }

        private Columns _gridColumns = null;

        /// <summary>
        /// Obtains the GridColumns property.
        /// </summary>
        public Columns GridColumns
        {
            get
            {
                return _gridColumns;
            }
        }

        private string _lastUpdate = string.Empty;

        /// <summary>
        /// Obtains the LastUpdate property.
        /// </summary>
        public string LastUpdate
        {
            get
            {
                return _lastUpdate;
            }
        }

        /// <summary>
        /// Constructor method. Builds an instance of MSDataGridHelperLayoutInfo with 
        /// the specified values for its properties.
        /// </summary>
        /// <param name="gridSplits">GridSplits property value.</param>
        /// <param name="gridColumns">GridColumns property value.</param>
        /// <param name="lastUpdate">LastUpdate property value.</param>
        public MSDataGridHelperLayoutInfo(Splits gridSplits, Columns gridColumns, string lastUpdate)
        {
            _gridSplits = gridSplits;
            _gridColumns = gridColumns;
            _lastUpdate = lastUpdate;
        }
    }

    /// <summary>
    /// Class to execute conversion between MSDataGridHelperLayoutInfo type and 
    /// other types so it can be serialized and displayed in the property grid of the designer.
    /// </summary>
    internal class MSDataGridHelperLayoutInfoConverter : TypeConverter
    {
        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, 
        /// using the specified context.
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
        /// <param name="destinationType">A Type that represents the type you want to convert to.</param>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
                return true;

            if (destinationType == typeof(string))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Returns whether this converter can convert an object of the given type to 
        /// the type of this converter, using the specified context. 
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
        /// <param name="sourceType">A Type that represents the type you want to convert from.</param>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return false;

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified 
        /// context and culture information. 
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
        /// <param name="culture">A CultureInfo. If nullNothingnullptra null reference 
        /// (Nothing in Visual Basic) is passed, the current culture is assumed.</param>
        /// <param name="value">The Object to convert.</param>
        /// <param name="destinationType">The Type to convert the value parameter to.</param>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            try
            {
                MSDataGridHelperLayoutInfo dgi = null;
                if (destinationType == typeof(InstanceDescriptor) && value is MSDataGridHelperLayoutInfo)
                {
                    dgi = (MSDataGridHelperLayoutInfo)value;

                    ConstructorInfo ctor = typeof(MSDataGridHelperLayoutInfo).GetConstructor(new Type[] { typeof(Splits), typeof(Columns), typeof(string) });
                    if (ctor != null)
                    {
                        return new InstanceDescriptor(ctor, new object[] { dgi.GridSplits, dgi.GridColumns, System.DateTime.Now.ToString() });
                    }
                }

                if (destinationType == typeof(string) && value is MSDataGridHelperLayoutInfo)
                {
                    return "[Grid Layout]";
                }
            }
            catch { }

            return base.ConvertTo(context, culture, value, destinationType);
        }

    }
}
