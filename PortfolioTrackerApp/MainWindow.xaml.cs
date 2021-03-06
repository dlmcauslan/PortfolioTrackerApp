﻿using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
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
		private List<string> mStockCodes;

		public MainWindow()
		{
			InitializeComponent();
			// Create database
			mDatabase = new DatabaseFunctions(DatabaseContract.PATH);
			// Create tables
			//mDatabase.DropTable(DatabaseContract.Purchases.TABLE);
			//mDatabase.DropTable(DatabaseContract.Dividends.TABLE);
			mDatabase.CreateTable(DatabaseContract.Purchases.CREATE_TABLE);
			mDatabase.CreateTable(DatabaseContract.Dividends.CREATE_TABLE);
			mDatabase.CreateTable(DatabaseContract.Historical.CREATE_TABLE);

			/** Here's what we want to do on startup */
			startupProcesses();

			// Testing charts
			String query = String.Format("WHERE {0} = '{1}' ORDER BY {2} DESC", DatabaseContract.Historical.CODE, "VEU.AX", DatabaseContract.Historical.DATE);
			String columns = String.Format("{0}, {1}", DatabaseContract.Historical.DATE, DatabaseContract.Historical.PRICE);
			DataTable testTable = mDatabase.SelectData(DatabaseContract.Historical.TABLE, columns, query);

			//ValuesA = new ChartValues<ObservablePoint>();
			//ValuesB = new ChartValues<ObservablePoint>();
			ValuesA = new ChartValues<DateTimePoint>();
			//DateTime today = DateTime.Today;
			//Labels = new string[5];
			//for (int i = 0; i < 20; i++)
			//{
			//	ValuesA.Add(new DateTimePoint(Utilities.toDateTime((string)testTable.Rows[i][DatabaseContract.Historical.DATE]),
			//		Convert.ToSingle(testTable.Rows[i].Field<Int64>(DatabaseContract.Purchases.PRICE)) / 100);
			//	//Labels[i] = today.AddDays(i).ToShortDateString();
			//	//ValuesA.Add(new ObservablePoint(i, i*i));
			//	//ValuesB.Add(new ObservablePoint(i, -i * i + 250));

			//}
			//foreach (DataRow row in testTable.Rows[])
			for (int i = 0; i<200; i++)
			{
				var row = testTable.Rows[i];
				DateTime date = Utilities.toDateTime(row.Field<String>(DatabaseContract.Historical.DATE));
				double price = Convert.ToSingle(row.Field<Int64>(DatabaseContract.Historical.PRICE)) / 100;
				if (price > 0)
				{
					ValuesA.Add(new DateTimePoint(date, price));
				}
			}
			Formatter = value => new System.DateTime((long)(value)).ToString("d");
			//Formatter = value => value.ToString("d");
			//Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" };
			DataContext = this;
		}

		public SeriesCollection SeriesCollection { get; set; }
		public ChartValues<DateTimePoint> ValuesA { get; set; }
		public Func<double, string> Formatter { get; set; }
		public string[] Labels { get; set; }
		//public ChartValues<ObservablePoint> ValuesA { get; set; }
		//public ChartValues<ObservablePoint> ValuesB { get; set; }

		/**
		 * Method called at startup to run all the backend stuff that needs to be done to get the
		 * app ready.
		 */
		private void startupProcesses()
		{
			// From the purchases table, select distinct stock codes, put into a List
			String stockCodeColumn = String.Format("DISTINCT {0}", DatabaseContract.Purchases.CODE);
			String rowOrdering = String.Format("ORDER BY {0} ASC", DatabaseContract.Purchases.CODE);
			mStockCodes = mDatabase.SelectData(DatabaseContract.Purchases.TABLE, stockCodeColumn, rowOrdering).AsEnumerable().Select(x => x[0].ToString()).ToList();

			// Update historical data for the stocks if required
			updateHistoricalData();


			// Populate tables from database
			updateMainTable();
			updatePurchaseTable();
			updateDividendsTable();
		}

		/**
		 * Updates the historical stock data if required - should happen at most once a day.
		 */
		private void updateHistoricalData()
		{
			// On opening the program get distinct stock-codes from purchases
			// database and update the historical data for each of them. Maybe even 
			// query the historical database to see what the most recent date is, to
			// avoid doing this check multiple times per day.
			// NEED loading screen probably
			String dateColumns = String.Format("MAX({0}) as MaxDate", DatabaseContract.Historical.DATE);
			DateTime maxDownloadedDate = Utilities.toDateTime(mDatabase.SelectData(DatabaseContract.Historical.TABLE, dateColumns).Rows[0]["MaxDate"].ToString());
			DateTime todaysDate = DateTime.Today;
			DayOfWeek todaysDay = todaysDate.DayOfWeek;
			int timeDifference = todaysDate.Subtract(maxDownloadedDate).Days;
			// The historical data needs to be updated if it is more than 2 days behind. Note the data is downloaded
			// from a US database, so the data is up to date if it is only 2 days behind. To account for weekends the data
			// needs to be more than 4 days behind on tuesday, more than 3 days behind on monday, more than 2 days behind
			// on sunday.
			if ((todaysDay.Equals(DayOfWeek.Tuesday) & timeDifference > 4) |
				(todaysDay.Equals(DayOfWeek.Monday) & timeDifference > 3) |
				((todaysDay.Equals(DayOfWeek.Sunday) | (int)todaysDay >= 3) & timeDifference > 2))     // Casting a DayOfWeek gives an integer from 0 (Sunday) to 6 (Saturday)
			{                   
				foreach (string stockCode in mStockCodes)
				{
					// Download stock data
					Downloader mDownloader = new Downloader(mDatabase, stockCode);
					mDownloader.download();
				}
			}
		}

		/**
		 * Updates the data in the main table on the front page.
		 */
		private void updateMainTable()
		{
			// Create a dataTable for main data.
			DataTable mainDataTable = new DataTable
			{
				Columns = {
					"stockCode", // typeof(string) is implied
					"stockName",
					{"price", typeof(float)},
					{"numberOwned", typeof(int)},
					{"spent", typeof(float)},
					{"stockValue", typeof(float)},
					{"dividendValue", typeof(float)},
					{"totalValue", typeof(float)},
					{"profit_$", typeof(float)},
					{"profit_%", typeof(float)}
				}
			};

			// Create a total "stock" which will hold the totals of totalSpent, totalDividend, totalStockValue, totalValue, profit% and profit$.
			float spentSum = 0;
			float dividendSum = 0;
			float stockValueSum = 0;
			float totalSum = 0;
			float profit = 0;

			// Loop over the different stock codes, creating a stock object for each one. Stock will have a code, name, currentPrice, totalNumberOwned, totalDividendValue, totalSpent. 
			// It will have methods to get currentPrice, totalNumberOwned, totalDividendValue, totalSpent, totalStockValue, totalOverallValue, profit%, and profit$.
			foreach (string stockCode in mStockCodes)
			{
				// Create a stock object and add it to the main datatable
				Stock stock = new Stock(stockCode, mDatabase);
				mainDataTable.Rows.Add(stock.getCode(),
					stock.getName(),
					stock.getCurrentPrice(),
					stock.getTotalNumberOwned(),
					stock.getTotalSpent(),
					stock.getTotalStockValue(),
					stock.getTotalDividend(),
					stock.getTotalOverallValue(),
					stock.getProfitDollar(),
					stock.getProfitPercent());
				// Calculate the overall sums
				spentSum += stock.getTotalSpent();
				dividendSum += stock.getTotalDividend();
				stockValueSum += stock.getTotalStockValue();
				totalSum += stock.getTotalOverallValue();
				profit += stock.getProfitDollar();
			}

			// Add a summary row
			mainDataTable.Rows.Add("Summary", null, null, null, spentSum, stockValueSum, dividendSum, totalSum, profit, profit / spentSum * 100);

			// From main data table populate the main table.
			dataGridMainTable.ItemsSource = mainDataTable.DefaultView;
		}

		/**
		 * Runs methods to update data on tab pages if different tabs are selected. 
		 */
		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (HomeTab.IsSelected) updateMainTable();
		}

		/*********************************
		 * Purchases tab methods
		 ********************************/

		/**
		 * Opens a new addPurchasePopup on clicking the AddPurchaseButton
		 */
		private void AddPurchaseButton_Click(object sender, RoutedEventArgs e)
		{
			AddPurchasePopup addPurchase = new AddPurchasePopup(mDatabase, AddPurchasePopup.WindowType.Purchases);
			// Updates the table if the dialog returns true. Note then line below actually opens th
			// dialog too.
			if (addPurchase.ShowDialog() == true) updatePurchaseTable(); 
		}

		/**
		 * Opens a new addPurchasePopup (configured as a sale) on clicking the AddSaleButton
		 */
		private void AddSaleButton_Click(object sender, RoutedEventArgs e)
		{
			AddPurchasePopup addPurchase = new AddPurchasePopup(mDatabase, AddPurchasePopup.WindowType.Sales);
			if (addPurchase.ShowDialog() == true) updatePurchaseTable();
		}

		/**
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

		/**
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

		/**
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

		/**
		 * Opens a new addDividendPopup on clicking the AddDividendButton
		 */
		private void AddDividendButton_Click(object sender, RoutedEventArgs e)
		{
			AddDividendPopup addDividend = new AddDividendPopup(mDatabase, AddDividendPopup.WindowType.AddDividend);
			// Updates the table if the dialog returns true. Note then line below actually opens the
			// dialog too.
			if (addDividend.ShowDialog() == true) updateDividendsTable();
		}

		/**
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

		/**
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

		/**
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

		/**
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
		}
	}
}
