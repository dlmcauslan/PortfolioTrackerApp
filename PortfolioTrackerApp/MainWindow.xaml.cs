using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PortfolioTrackerApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public DatabaseFunctions mDatabase;
		private DataTable mPurchasesTable;
		private DataTable mDividendsTable;

		

		public MainWindow()
		{
			InitializeComponent();
			// Create database
			mDatabase = new DatabaseFunctions(DatabaseContract.PATH);
			// Create tables
			//mDatabase.DropTable(DatabaseContract.Purchases.TABLE);
			mDatabase.CreateTable(DatabaseContract.Purchases.CREATE_TABLE);
			mDatabase.CreateTable(DatabaseContract.Dividends.CREATE_TABLE);
			mDatabase.CreateTable(DatabaseContract.Historical.CREATE_TABLE);

			// Uncomment to add some test data to the database
			//testDatabase(mDatabase);

			// Populate tables from database
			updatePurchaseTable();
			updateDividendsTable();
		}

		/*********************************
		 * Purchases tab methods
		 ********************************/

		/*
		 * Opens a new addPurchasePopup on clicking the AddPurchaseButton
		 */
		private void AddPurchaseButton_Click(object sender, RoutedEventArgs e)
		{
			AddPurchasePopup addPurchase = new AddPurchasePopup(mDatabase, AddPurchasePopup.WindowType.Purchases);
			// Updates the table if the dialog returns true. Note then line below actually opens th
			// dialog too.
			if (addPurchase.ShowDialog() == true) updatePurchaseTable(); 
		}

		/*
		 * Opens a new addPurchasePopup (configured as a sale) on clicking the AddSaleButton
		 */
		private void AddSaleButton_Click(object sender, RoutedEventArgs e)
		{
			AddPurchasePopup addPurchase = new AddPurchasePopup(mDatabase, AddPurchasePopup.WindowType.Sales);
			if (addPurchase.ShowDialog() == true) updatePurchaseTable();
		}

		/*
		 * Opens a new addPurchasePopup (configured as an edit purchase) on clicking the EditPurchaseButton
		 */
		private void EditPurchaseButton_Click(object sender, RoutedEventArgs e)
		{
			DataRowView selectedRow = (DataRowView)dataGridPurchases.SelectedItem;
			if (selectedRow == null)
			{
				MessageBox.Show("No purchase selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			else
			{
				AddPurchasePopup addPurchase = new AddPurchasePopup(mDatabase, AddPurchasePopup.WindowType.EditPurchase, selectedRow);
				if (addPurchase.ShowDialog() == true) updatePurchaseTable();
			}
		}

		/*
		 * Asks the user if they want to delete a purchase from the database.
		 */
		private void DeletePurchaseButton_Click(object sender, RoutedEventArgs e)
		{
			DataRowView selectedRow = (DataRowView)dataGridPurchases.SelectedItem;
			if (selectedRow == null)
			{
				MessageBox.Show("No purchase selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			else
			{
				MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this purchase?", "Delete purchase?", MessageBoxButton.YesNo);
				if (result == MessageBoxResult.Yes)
				{
					Int64 rowID = selectedRow.Row.Field<Int64>(DatabaseContract.Purchases.ID);
					mDatabase.DeleteData(DatabaseContract.Purchases.TABLE, DatabaseContract.Purchases.ID + " = " + rowID);		
				}
				updatePurchaseTable();
			}
		}

		/*
		 * Updates the purchases datagrid from the sql database.
		 * Creates Price in $ column and Total cost column.
		 * Call this method anytime the purchases database has been modified
		 */
		private void updatePurchaseTable()
		{
			// Select data from database
			mPurchasesTable = mDatabase.SelectData(DatabaseContract.Purchases.TABLE);
			// Add Price (in dollars) column and Total cost column to datatable
			String priceColumn = "Price_$";
			String totalColumn = "Total_Cost";
			mPurchasesTable.Columns.Add(new DataColumn(priceColumn, typeof(float)));
			mPurchasesTable.Columns.Add(new DataColumn(totalColumn, typeof(float)));
			foreach (DataRow row in mPurchasesTable.Rows)
			{
				row.SetField(priceColumn, Convert.ToSingle(row.Field<Int64>(DatabaseContract.Purchases.PRICE)) / 100);
				row.SetField(totalColumn, row.Field<float>(priceColumn) * Convert.ToSingle(row.Field<Int64>(DatabaseContract.Purchases.NUMBER)));
			}
			// Use the datatable as the data source for the datagrid
			dataGridPurchases.ItemsSource = mPurchasesTable.DefaultView;
		}


		/*********************************
		 * Dividends tab methods
		 ********************************/

		/*
		 * Opens a new addDividendPopup on clicking the AddDividendButton
		 */
		private void AddDividendButton_Click(object sender, RoutedEventArgs e)
		{
			AddDividendPopup addDividend = new AddDividendPopup(mDatabase, AddDividendPopup.WindowType.AddDividend);
			// Updates the table if the dialog returns true. Note then line below actually opens the
			// dialog too.
			if (addDividend.ShowDialog() == true) updateDividendsTable();
		}

		/*
		 * Opens a new addDividendPopup (configured as and edit dividend popup) on clicking the EditDividendButton
		 */
		private void EditDividendButton_Click(object sender, RoutedEventArgs e)
		{
			DataRowView selectedRow = (DataRowView)dataGridDividends.SelectedItem;
			if (selectedRow == null)
			{
				MessageBox.Show("No dividend selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			else
			{
				AddDividendPopup addDividend = new AddDividendPopup(mDatabase, AddDividendPopup.WindowType.EditDividend, selectedRow);
				if (addDividend.ShowDialog() == true) updateDividendsTable();
			}
		}

		/*
		 * Opens a warning box to make sure the user wishes to delete the selected item.
		 */
		private void DeleteDividendButton_Click(object sender, RoutedEventArgs e)
		{
			DataRowView selectedRow = (DataRowView)dataGridDividends.SelectedItem;
			if (selectedRow == null)
			{
				MessageBox.Show("No dividend selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			else
			{
				MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this dividend?", "Delete dividend?", MessageBoxButton.YesNo);
				if (result == MessageBoxResult.Yes)
				{
					Int64 rowID = selectedRow.Row.Field<Int64>(DatabaseContract.Dividends.ID);
					mDatabase.DeleteData(DatabaseContract.Dividends.TABLE, DatabaseContract.Dividends.ID + " = " + rowID);
				}
				updateDividendsTable();
			}
		}

		/*
		 * Updates the dividends datagrid from the sql database.
		 * Creates Amount in $ column
		 * Call this method anytime the dividends database has been modified
		 */
		private void updateDividendsTable()
		{
			// Select data from database
			mDividendsTable = mDatabase.SelectData(DatabaseContract.Dividends.TABLE);
			// Add Amount (in dollars) column - probably need a check in here if column doesn't already exist. 
			// OR create a new method to create tables.
			String newColumn = "Amount_$";
			mDividendsTable.Columns.Add(new DataColumn(newColumn, typeof(float)));
			foreach (DataRow row in mDividendsTable.Rows)
			{
				row.SetField(newColumn, Convert.ToSingle(row.Field<Int64>(DatabaseContract.Dividends.AMOUNT)) / 100);
			}
			// Use the datatable as the data source for the datagrid
			dataGridDividends.ItemsSource = mDividendsTable.DefaultView;
		}

		/*
		 * A bunch of statements for database testing
		 */
		private void testDatabase(DatabaseFunctions database)
		{
			//database.droptable(databasecontract.Historical.TABLE);
			//String testPurchasesValues = String.Format("'VAS', '27/06/2016', {0}, {1}", 32, 1234);
			//int insertionSuccessful = database.InsertData(DatabaseContract.Purchases.TABLE, DatabaseContract.Purchases.COLUMNS, testPurchasesValues);
			//Console.WriteLine(insertionSuccessful);

			String testPurchasesDelete = DatabaseContract.Purchases.ID + " = 40";
			int deleteSuccessful = database.DeleteData(DatabaseContract.Purchases.TABLE, testPurchasesDelete);
			if (deleteSuccessful > 0) Console.WriteLine("Delete successful");
			else Console.WriteLine("Delete failed");

			String testPurchaseSet = DatabaseContract.Purchases.CODE + "= 'IJR', " + DatabaseContract.Purchases.NUMBER + "= 100 ";
			String testCondition = DatabaseContract.Purchases.ID + " = 5";
			int updateSuccess = database.EditData(DatabaseContract.Purchases.TABLE, testPurchaseSet, testCondition);
			Console.WriteLine(updateSuccess);

			// This doesn't work because need to close the database!!!!
			//String testCols = "*";
			//SQLiteDataReader reader = database.SelectData(DatabaseContract.Purchases.TABLE, testCols, "");
			//while (reader.Read())
			//{
			//	Console.WriteLine(reader["_ID"] + " " + reader[DatabaseContract.Purchases.CODE]);
			//}

			
		}

		

	}
}
