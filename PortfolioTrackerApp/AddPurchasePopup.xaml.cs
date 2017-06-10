using System;
using System.Collections.Generic;
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
		public AddPurchasePopup(DatabaseFunctions database)
		{
			InitializeComponent();
			mDatabase = database;
		}

		private void buttonDialogOK_Click(object sender, RoutedEventArgs e)
		{
			// Check that the data is ok
			// Check stock code
			if (textboxStockCode.Text == "")
			{
				MessageBox.Show("Purchase must have a stock code.", "Data Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			// Check number purchased
			if (!(int.TryParse(textboxNumberPurchased.Text, out int numberPurchasedInt)) | numberPurchasedInt <= 0)
			{
				MessageBox.Show("Number purchased must be a number and also be greater than 0.", "Data Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

			// Add the data to the Purchases database
			String purchasesValues = String.Format("'{0}', '{1}', {2}, {3}", textboxStockCode.Text, textboxDate.Text, numberPurchasedInt, Utilities.DollarsToCents(priceDollars));
			Console.WriteLine(purchasesValues);
			int insertionSuccessful = mDatabase.InsertData(DatabaseContract.Purchases.TABLE, DatabaseContract.Purchases.COLUMNS, purchasesValues);
			Console.WriteLine(insertionSuccessful);
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
