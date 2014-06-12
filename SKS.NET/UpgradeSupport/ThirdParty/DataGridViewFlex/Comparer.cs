using System;
using System.Windows.Forms;
namespace UpgradeHelpers.Windows.Forms
{

    /// <summary>
    /// Comparer class that can be used with the Flex Grid behaviour of the ExtendedDataGridView.
    /// </summary>
    public class FlexComparer : System.Collections.IComparer
    {
        private SortSettings sortSetting = SortSettings.SortGenericAscending;
        private int sortOrder = 1;
        /// <summary>
        /// Creates a FlexComparer with the specified sort order.
        /// </summary>
        /// <param name="sortOrder">The sort order to use.</param>
        public FlexComparer(SortSettings sortOrder)
        {
            sortSetting = sortOrder;
            this.sortOrder = (sortOrder == SortSettings.SortGenericAscending ||
                        sortOrder == SortSettings.SortNumericAscending ||
                        sortOrder == SortSettings.SortStringAscending ||
                        sortOrder == SortSettings.SortStringNoCaseAscending) ? 1 : -1;
        }

        /// <summary>
        /// Compares two objects using the internal sort order.
        /// </summary>
        /// <param name="x">The first object</param>
        /// <param name="y">The second object</param>
        /// <returns>The difference in values between the items.  If they are equal, returns 0.</returns>
        public int Compare(object x, object y)
        {
            DataGridViewRow DataGridViewRow1 = (DataGridViewRow)x;
            DataGridViewRow DataGridViewRow2 = (DataGridViewRow)y;
            int CompareResult = 0;
            int currentColumnIndex = 0;


            if (DataGridViewRow1.DataGridView.SelectedCells.Count > 0)
            {
                //set the minimum valid value to the currentColumnIndex
                currentColumnIndex = Math.Max(0, DataGridViewRow1.DataGridView.SelectedCells[0].ColumnIndex);
                //check the maximum valid value to the currentColumnIndex
                currentColumnIndex = Math.Min(currentColumnIndex, DataGridViewRow1.DataGridView.Columns.Count);
            }

            String value1 = String.Empty;
            if (DataGridViewRow1.Cells[currentColumnIndex].Value != null)
            {
                value1 = DataGridViewRow1.Cells[currentColumnIndex].Value.ToString();
            }
            else
                return -1 * sortOrder;

            String value2 = String.Empty;
            if (DataGridViewRow2.Cells[currentColumnIndex].Value != null)
            {
                value2 = DataGridViewRow2.Cells[currentColumnIndex].Value.ToString();
            }
            else return 1 * sortOrder;

            Double tempGeneric1 = 0.0;
            Double tempGeneric2 = 0.0;
            bool value1IsNumeric = false;
            bool value2IsNumeric = false;

            switch (sortSetting)
            {
                case SortSettings.SortNone:
                    CompareResult = 0;
                    break;
                case SortSettings.SortGenericAscending:
                case SortSettings.SortGenericDescending:
                    value1IsNumeric = Double.TryParse(value1, out tempGeneric1);
                    value2IsNumeric = Double.TryParse(value2, out tempGeneric2);
                    if (value1IsNumeric && value2IsNumeric)
                    {
                        CompareResult = tempGeneric1 > tempGeneric2 ? 1 : tempGeneric1 < tempGeneric2 ? -1 : 0;
                    }
                    else if (value1IsNumeric && !value2IsNumeric)
                    {
                        CompareResult = -1;
                    }
                    else if (!value1IsNumeric && value2IsNumeric)
                    {
                        CompareResult = 1;
                    }
                    else
                    {
                        CompareResult = System.String.Compare(value1, value2);
                    }
                    break;
                case SortSettings.SortStringAscending:
                case SortSettings.SortStringDescending:
                    CompareResult = System.String.Compare(value1, value2);
                    break;
                case SortSettings.SortStringNoCaseAscending:
                case SortSettings.SortStringNoCaseDescending:
                    CompareResult = System.String.Compare(value1, value2, true);
                    break;
                case SortSettings.SortNumericAscending:
                case SortSettings.SortNumericDescending:
                    value1IsNumeric = Double.TryParse(value1, out tempGeneric1);
                    value2IsNumeric = Double.TryParse(value2, out tempGeneric2);
                    if (value1IsNumeric && value2IsNumeric)
                    {
                        CompareResult = tempGeneric1 > tempGeneric2 ? 1 : tempGeneric1 < tempGeneric2 ? -1 : 0;
                    }
                    else if (value1IsNumeric && !value2IsNumeric)
                    {
                        CompareResult = 1;
                    }
                    else if (!value1IsNumeric && value2IsNumeric)
                    {
                        CompareResult = -1;
                    }
                    else
                    {
                        CompareResult = 0;
                    }
                    break;
            }
            return CompareResult * sortOrder;
        }
    }
}