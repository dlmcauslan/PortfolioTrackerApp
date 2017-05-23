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

namespace PortfolioTrackerApp
{
	class DatabaseContract
	{
		// The database path
		public const String PATH = "PortfolioDatabase.sqlite";

		/**
		 * Class to hold the database contract for the Purchases table which
		 * contains a record of all of the stock purchases and sales that I have
		 * made.
		 */
		abstract class Purchases
		{
			// The table name
			public const String TABLE = "Purchases";

			// Columns 
			public const String ID = "_ID";
			public const String CODE = "Stock_Code";
			public const String NUMBER = "Number_Purchased";
			public const String DATE = "Date";
			public const String PRICE = "Price";
		}

		/**
		 * Class to hold the contract for the Dividends table which contains a
		 * record of all of the dividend payments that I have recieved.
		 */
		abstract class Dividends
		{
			// The table name
			public const String TABLE = "Dividends";

			// Columns
			public const String ID = "_ID";
			public const String CODE = "Stock_Code";
			public const String DATE = "Date";
			public const String AMOUNT = "Dividend_Amount";
		}

		/**
		 * Class to hold the contract for the Historical_Data table which contains
		 * the historical price data of the stocks in my portfolio.
		 */
		abstract class Historical
		{
			// The table name
			public const String TABLE = "Historical_Data";

			// Columns
			public const String PK = TABLE + "_PK";
			public const String CODE = "Stock_Code";
			public const String DATE = "Date";
			public const String PRICE = "Price";
		}
	}
}
