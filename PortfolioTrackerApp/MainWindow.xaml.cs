using System;
using System.Collections.Generic;
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

		public MainWindow()
		{
			InitializeComponent();
			// Create database
			mDatabase = new DatabaseFunctions(DatabaseContract.PATH);
			// Create tables
			//database.DropTable(DatabaseContract.Purchases.TABLE);
			mDatabase.CreateTable(DatabaseContract.Purchases.CREATE_TABLE);
			mDatabase.CreateTable(DatabaseContract.Dividends.CREATE_TABLE);
			mDatabase.CreateTable(DatabaseContract.Historical.CREATE_TABLE);

			// Uncomment to add some test data to the database
			testDatabase(mDatabase);
		}

		private void AddPurchaseButton_Click(object sender, RoutedEventArgs e)
		{
			Console.WriteLine("Here");
			//MessageBox.Show("Hello, world!");
			AddPurchasePopup addPurchase = new AddPurchasePopup(mDatabase);
			addPurchase.ShowDialog();
		}

		private void AddSaleButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void EditPurchaseButton_Click(object sender, RoutedEventArgs e)
		{

		}

		/**
		 * A bunch of statements for database testing
		 */
		private void testDatabase(DatabaseFunctions database)
		{
			//database.droptable(databasecontract.Historical.TABLE);
			String testPurchasesValues = String.Format("'VAS', '27/06/2016', {0}, {1}", 32, 1234);
			int insertionSuccessful = database.InsertData(DatabaseContract.Purchases.TABLE, DatabaseContract.Purchases.COLUMNS, testPurchasesValues);
			Console.WriteLine(insertionSuccessful);

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
