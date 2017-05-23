/** DatabaseContract
 * 
 * A contract for the sqlite database that contains the portfolio
 * and historical stock price data.
 * 
 * Created 23/05/2017 DLM
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTrackerApp.Database_Files
{
	class DatabaseContract
	{
		// The database path
		public const string PATH = "Data/PortfolioDatabase.db";

		/**
		 * Class to hold the database contract for the Purchases table which
		 * contains a record of all of the stock purchases and sales that I have
		 * made.
		 */
		abstract class Purchases
		{
			// The table name
			public const string TABLE = "Purchases";

			// Columns 
			public const string ID = "_ID";
			public const string CODE = "Stock_Code";
			public const string NUMBER = "Number_Purchased";
			public const string DATE = "Date";
			public const string PRICE = "Price";
		}

		/**
		 * Class to hold the contract for the Dividends table which contains a
		 * record of all of the dividend payments that I have recieved.
		 */
		 abstract class Dividends
		{
			// The table name
			public const string TABLE = "Dividends";

			// Columns
			public const string ID = "_ID";
			public const string CODE = "Stock_Code";
			public const string DATE = "Date";
			public const string AMOUNT = "Dividend_Amount";
		}

		/**
		 * Class to hold the contract for the Historical_Data table which contains
		 * the historical price data of the stocks in my portfolio.
		 */
		 abstract class Historical
		{
			// The table name
			public const string TABLE = "Historical_Data";

			// Columns
			public const string PK = TABLE + "_PK";
			public const string CODE = "Stock_Code";
			public const string DATE = "Date";
			public const string PRICE = "Price";
		}
	}
}
