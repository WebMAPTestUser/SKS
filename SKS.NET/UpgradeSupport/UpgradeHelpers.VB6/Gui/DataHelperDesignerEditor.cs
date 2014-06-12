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
    /// Class to implement the custom editor of the property DataHelperBinding, inherits UITypeEditor.
    /// </summary>
    internal class DataHelperDesignerEditor : UITypeEditor
    {
        private IWindowsFormsEditorService editorService = null;

        /// <summary>
        /// Informs what will be the type of designer to be used, in this case it will be modal.
        /// </summary>
        /// <param name="context">System.ComponentModel.ITypeDescriptorContext.</param>
        /// <returns>UITypeEditorEditStyle.Modal.</returns>
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        /// <summary>
        /// Takes care of displaying the modal window to edit the current value. 
        /// For this property it will use an  instance of DataHelperBindingInfo 
        /// to exchange the information.
        /// </summary>
        /// <param name="context">ITypeDescriptorContext.</param>
        /// <param name="provider">IServiceProvider.</param>
        /// <param name="value">Object.</param>
        /// <returns></returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            DataHelperDesignerModalView selectionControl = null;
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
                    selectionControl = new DataHelperDesignerModalView((DataHelperBindingInfo)value, context);
                else
                    selectionControl = new DataHelperDesignerModalView(DataHelper.getDefaultDataHelperBindingInfo((Control)context.Instance), context);

                if (editorService.ShowDialog(selectionControl) == DialogResult.OK)
                    value = selectionControl.DataHelperBindingInfo;
            }

            return value;
        }
    }

    /// <summary>
    /// Class to store information about what datahelper is going to be used to bind and 
    /// what extra parameters are required.
    /// </summary>
    [TypeConverter(typeof(DataHelperBindingInfoConverter))]
    public class DataHelperBindingInfo
    {
        private DataHelper _bindingDataHelper = null;
        /// <summary>
        /// The DataHelper control that will be used to bind the control.
        /// </summary>
        public DataHelper BindingControl
        {
            get
            {
                return _bindingDataHelper;
            }
        }

        private List<KeyValuePair<string, string>> _bindingParameters = new List<KeyValuePair<string, string>>();
        /// <summary>
        /// The list of parameters that should be provided. 
        /// This list plus the type of the control being binding will be used 
        /// to determinate what BindControl function will be invoked.
        /// </summary>
        public List<KeyValuePair<string, string>> BindingParameters
        {
            get
            {
                return _bindingParameters;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="bindingControl">DataHelper.</param>
        /// <param name="bindingParameters">List of parameters.</param>
        public DataHelperBindingInfo(DataHelper bindingControl, KeyValuePair<string, string>[] bindingParameters)
        {
            this._bindingDataHelper = bindingControl;
            if (bindingParameters != null)
            {
                foreach (KeyValuePair<string, string> param in bindingParameters)
                {
                    _bindingParameters.Add(param);
                }
            }
        }
    }

    /// <summary>
    /// Class to execute conversion between DataHelperBindingInfo type and other types so 
    /// it can be serialized and displayed in the property grid of the designer.
    /// </summary>
    internal class DataHelperBindingInfoConverter : TypeConverter
    {
        /// <summary>
        /// To what types can DataHelperBindingInfo be converted.
        /// </summary>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
                return true;

            if (destinationType == typeof(string))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// From what types can DataHelperBindingInfo be converted.
        /// </summary>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return false;

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Executes the convertion.
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context,
        CultureInfo culture, object value, Type destinationType)
        {
            try
            {
                DataHelperBindingInfo dh = null;
                if (destinationType == typeof(InstanceDescriptor) && value is DataHelperBindingInfo)
                {
                    dh = (DataHelperBindingInfo)value;

                    ConstructorInfo ctor = typeof(DataHelperBindingInfo).GetConstructor(new Type[] { typeof(DataHelper), typeof(KeyValuePair<string, string>[]) });
                    if (ctor != null)
                    {
                        return new InstanceDescriptor(ctor, new object[] { dh.BindingControl, dh.BindingParameters.ToArray() });
                    }
                }

                if (destinationType == typeof(string) && value is DataHelperBindingInfo)
                {
                    dh = (DataHelperBindingInfo)value;
                    if (dh.BindingControl != null)
                        return dh.BindingControl.Text + " [" + dh.BindingControl.Name + "]";
                    else
                        return "";
                }
            }
            catch { }

            return base.ConvertTo(context, culture, value, destinationType);
        }

    }
}
