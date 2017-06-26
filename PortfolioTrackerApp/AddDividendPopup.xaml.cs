using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PortfolioTrackerApp
{
	/// <summary>
	/// Interaction logic for AddDividendPopup.xaml
	/// </summary>
	public partial class AddDividendPopup : Window
	{
		private DatabaseFunctions mDatabase;
		public enum WindowType { AddDividend, EditDividend };
		private WindowType mAddEdit;
		private DataRowView mRowData;

		public AddDividendPopup(DatabaseFunctions database, WindowType addEdit, DataRowView rowData = null)
		{
			InitializeComponent();
			mDatabase = database;
			mAddEdit = addEdit;
			mRowData = rowData;
			// Update the dialog depending on whether the WindowType is Add or Edit Dividend.
			String[] windowTitle = { "Add Dividend", "Edit Dividend" };
			Title = windowTitle[(int)mAddEdit];

			// If its an Edit Dividend window fill the data fields with the imported rowData
			if (mAddEdit == WindowType.EditDividend)
			{
				textboxStockCode.Text = mRowData.Row.Field<String>(DatabaseContract.Dividends.CODE);
				textboxDate.Text = mRowData.Row.Field<String>(DatabaseContract.Dividends.DATE);
				textboxAmount.Text = mRowData.Row.Field<float>("Amount_$").ToString();
			}
		}

		private void buttonDialogOK_Click(object sender, RoutedEventArgs e)
		{
			// Check that the data is ok
			// Check stock code
			if (textboxStockCode.Text == "")
			{
				MessageBox.Show("Dividend must have a stock code.", "Data Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			// Check amount
			if (!(float.TryParse(textboxAmount.Text, out float priceDollars)) | priceDollars <= 0)
			{
				MessageBox.Show("Dividend amount must be a number in $ that is greater than 0.", "Data Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			// Check Date
			if (!Utilities.checkDate(textboxDate.Text))
			{
				MessageBox.Show("Invalid date.", "Data Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			// TODO
			// Check that stock is in historical database, if it isn't download the historical
			// data for that stock

			// Add the data to the Dividends database
			int databaseActionSuccessful = 0;
			if (mAddEdit == WindowType.EditDividend)
			{
				String setStatement = DatabaseContract.Dividends.CODE + " = '" + textboxStockCode.Text.ToUpper()
								+ ", " + DatabaseContract.Dividends.DATE + " = '" + textboxDate.Text
								+ "', " + DatabaseContract.Dividends.AMOUNT + " = " + Utilities.DollarsToCents(priceDollars);
				String whereStatement = DatabaseContract.Dividends.ID + " = " + mRowData.Row.Field<Int64>(DatabaseContract.Dividends.ID).ToString();
				databaseActionSuccessful = mDatabase.EditData(DatabaseContract.Dividends.TABLE, setStatement, whereStatement);
			}
			else
			{
				String dividendsValues = String.Format("'{0}', '{1}', {2}", textboxStockCode.Text.ToUpper(), textboxDate.Text, Utilities.DollarsToCents(priceDollars));
				databaseActionSuccessful = mDatabase.InsertData(DatabaseContract.Dividends.TABLE, DatabaseContract.Dividends.COLUMNS, dividendsValues);
			}
			// If all is succesful close the dialog
			this.DialogResult = true;
		}

		private void Window_ContentRendered(object sender, EventArgs e)
		{
			// Automatically focus' on the StockCode text box
			textboxStockCode.Focus();
		}
	}

}
