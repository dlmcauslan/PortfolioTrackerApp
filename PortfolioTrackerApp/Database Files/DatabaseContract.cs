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
		public const String PATH = "PortfolioDatabase.db";

		/**
		 * Class to hold the database contract for the Purchases table which
		 * contains a record of all of the stock purchases and sales that I have
		 * made.
		 */
		 public abstract class Purchases
		{
			// The table name
			public const String TABLE = "Purchases";

			// Columns 
			public const String ID = "_ID";
			public const String CODE = "Stock_Code";
			public const String NUMBER = "Number_Purchased";
			public const String DATE = "Date"; // text in form yyyy-dd-mm
			public const String PRICE = "Price"; // integer in cents
			public const String COLUMNS = CODE + ", " + DATE + ", " + NUMBER + ", " + PRICE;

			// Table create string
			public const String CREATE_TABLE = "CREATE TABLE IF NOT EXISTS " + TABLE + " (" +
				ID + " INTEGER PRIMARY KEY AUTOINCREMENT, " + CODE + " TEXT NOT NULL, " + 
				NUMBER + " INTEGER NOT NULL, " + DATE + " TEXT NOT NULL, " + PRICE + 
				" INTEGER NOT NULL)";
				
		}

		/**
		 * Class to hold the contract for the Dividends table which contains a
		 * record of all of the dividend payments that I have recieved.
		 */
		public abstract class Dividends
		{
			// The table name
			public const String TABLE = "Dividends";

			// Columns
			public const String ID = "_ID";
			public const String CODE = "Stock_Code";
			public const String DATE = "Date";		// In the format yyyy-mm-dd
			public const String AMOUNT = "Dividend_Amount";     // In cents
			public const String COLUMNS = CODE + ", " + DATE + ", " + AMOUNT;


			// Table create string
			public const String CREATE_TABLE = "CREATE TABLE IF NOT EXISTS " + TABLE + " (" +
				ID + " INTEGER PRIMARY KEY AUTOINCREMENT, " + CODE + " TEXT NOT NULL, " +
				DATE + " TEXT NOT NULL, " + AMOUNT + " INTEGER NOT NULL)";
		}

		/**
		 * Class to hold the contract for the Historical_Data table which contains
		 * the historical price data of the stocks in my portfolio.
		 */
		public abstract class Historical
		{
			// The table name
			public const String TABLE = "Historical_Data";

			// Columns
			public const String PK = TABLE + "_PK";
			public const String CODE = "Stock_Code";
			public const String DATE = "Date";		// in the format yyyy-mm-dd
			public const String PRICE = "Price";    // in cents
			public const String COLUMNS = CODE + ", " + DATE + ", " + PRICE;


			// Create table string
			public const String CREATE_TABLE = "CREATE TABLE IF NOT EXISTS " + TABLE + " (" +
				CODE + " TEXT NOT NULL, " + DATE + " TEXT NOT NULL, " + PRICE + " INTEGER NOT NULL, " +
				"CONSTRAINT " + PK + " PRIMARY KEY (" + CODE + ", "+ DATE + ", " + PRICE + "))";
		}
	}
}
