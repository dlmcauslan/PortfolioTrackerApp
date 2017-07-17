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
	/// Interaction logic for AddPurchasePopup.xaml
	/// </summary>
	public partial class AddPurchasePopup : Window
	{
		private DatabaseFunctions mDatabase;
		public enum WindowType { Purchases, Sales, EditPurchase };
		private WindowType mBuySellEdit;
		private DataRowView mRowData;
		public AddPurchasePopup(DatabaseFunctions database, WindowType buySellEdit, DataRowView rowData = null)
		{
			InitializeComponent();
			mDatabase = database;
			mBuySellEdit = buySellEdit;
			mRowData = rowData;
			// Update the dialog depending on whether the WindowType is Purchases, Sales or EditPurchase.
			String[] windowTitle = { "Add Purchase", "Add Sale", "Edit Purchase/Sale" };
			String[] purchasedContent = { "Number Purchased", "Number Sold", "Number Purchased/Sold" };
			Title = windowTitle[(int)mBuySellEdit];
			labelNumberPurchased.Content = purchasedContent[(int)mBuySellEdit];

			// If its an Edit Purchase window fill the data fields with the imported rowData
			if (mBuySellEdit == WindowType.EditPurchase)
			{
				textboxStockCode.Text =mRowData.Row.Field<String>(DatabaseContract.Purchases.CODE);
				textboxNumberPurchased.Text = mRowData.Row.Field<Int64>(DatabaseContract.Purchases.NUMBER).ToString();
				textboxDate.Text = Utilities.convertDate(mRowData.Row.Field<String>(DatabaseContract.Purchases.DATE));
				textboxPrice.Text = mRowData.Row.Field<float>("Price_$").ToString();

			}
		}

		private void buttonDialogOK_Click(object sender, RoutedEventArgs e)
		{
			// Check that the data is ok
			// Check stock code
			String string1 = "Purchase";
			if (mBuySellEdit.Equals(WindowType.Sales)) string1 = "Sale";
			if (textboxStockCode.Text == "")
			{
				MessageBox.Show(string1 + " must have a stock code.", "Data Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			// Check number purchased
			String string2 = "purchased";
			if (mBuySellEdit.Equals(WindowType.Sales)) string2 = "sold";
			if (!(int.TryParse(textboxNumberPurchased.Text, out int numberPurchasedInt)))
			{
				MessageBox.Show("Number " + string2 +" must be a number.", "Data Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			if (mBuySellEdit != WindowType.EditPurchase & numberPurchasedInt <= 0)
			{
				MessageBox.Show("Number " + string2 + " must be greater than 0.", "Data Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			// Check price
			if (!(float.TryParse(textboxPrice.Text, out float priceDollars)) | priceDollars < 0)
			{
				MessageBox.Show("Price must be a number and also be >= 0.", "Data Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			// Check Date
			if (!Utilities.checkDate(textboxDate.Text))
			{
				MessageBox.Show("Invalid date.", "Data Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			// Check that stock is in historical database, if it isn't download the historical
			// data for that stock
			String stockSymbol = textboxStockCode.Text.ToUpper();
			String query = String.Format("WHERE {0} = '{1}'", DatabaseContract.Historical.CODE, stockSymbol);
			DataTable queryResult = mDatabase.SelectData(DatabaseContract.Historical.TABLE, "*", query);
			if(queryResult.Rows.Count == 0)
			{
				Downloader mDownloader = new Downloader(mDatabase, stockSymbol);
				mDownloader.download();
			}

			// Add the data to the Purchases database
			if (mBuySellEdit.Equals(WindowType.Sales)) numberPurchasedInt = -numberPurchasedInt; // If its a sale, negate the number (sold) when adding to the database.
			// If window is an edit purchase call EditData, otherwise call InsertData
			int databaseActionSuccessful = 0;
			if (mBuySellEdit == WindowType.EditPurchase)
			{
				String setStatement = DatabaseContract.Purchases.CODE + " = '" + stockSymbol
								+ "', " + DatabaseContract.Purchases.NUMBER + " = " + numberPurchasedInt
								+ ", " + DatabaseContract.Purchases.DATE + " = '" + Utilities.convertDateReverse(textboxDate.Text)
								+ "', " + DatabaseContract.Purchases.PRICE + " = " + Utilities.DollarsToCents(priceDollars);
				String whereStatement = DatabaseContract.Purchases.ID + " = " + mRowData.Row.Field<Int64>(DatabaseContract.Purchases.ID).ToString();
				databaseActionSuccessful = mDatabase.EditData(DatabaseContract.Purchases.TABLE, setStatement, whereStatement);
			}
			else
			{
				String purchasesValues = String.Format("'{0}', '{1}', {2}, {3}", stockSymbol, Utilities.convertDateReverse(textboxDate.Text), numberPurchasedInt, Utilities.DollarsToCents(priceDollars));
				databaseActionSuccessful = mDatabase.InsertData(DatabaseContract.Purchases.TABLE, DatabaseContract.Purchases.COLUMNS, purchasesValues);
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
