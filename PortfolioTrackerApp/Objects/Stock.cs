using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTrackerApp
{
	public class Stock
	{
		// Setup class variables
		private readonly string mCode;
		private readonly string mName;
		private DatabaseFunctions mDatabase;
		private double mCurrentPrice;
		private int mTotalNumberOwned;
		private double mTotalDividendValue;
		private double mTotalSpent;

		public Stock(string code, DatabaseFunctions database)
		{
			// Initialize class
			mCode = code;
			mName = code;       // This is temporary, will create another database for codes and names later
			mDatabase = database;

			// These don't work yet. Need to fix!
			setCurrentPrice();
			setTotalNumberOwned();
			setTotalDividend();
			setTotalSpent();
		}

		/*
		 * Sets the current stock price by accessing the historical database
		 */
		private void setCurrentPrice()
		{
			// Get the most recent date that is already in the historical data table for the particular stock and get the price at this date
			String query = String.Format("WHERE {0} = '{1}' ORDER BY {2} DESC LIMIT 1 ", DatabaseContract.Historical.CODE, mCode, DatabaseContract.Historical.DATE);
			String selectStatement = String.Format("{0}, {1}", DatabaseContract.Historical.DATE, DatabaseContract.Historical.PRICE);
			DataTable queryResult = mDatabase.SelectData(DatabaseContract.Historical.TABLE, selectStatement, query);
			mCurrentPrice = Convert.ToSingle(queryResult.Rows[0].Field<Int64>(DatabaseContract.Historical.PRICE))/100;
		}

		/*
		 * Sets the total number owned of the stock by accesssing the Purchases database
		 */
		private void setTotalNumberOwned()
		{
			// Gets the sum of the number of shares bought from the purchases table
			String query = String.Format("WHERE {0} = '{1}'", DatabaseContract.Purchases.CODE, mCode);
			String selectStatement = String.Format("SUM({0}) AS TotalOwned", DatabaseContract.Purchases.NUMBER);
			DataTable queryResult = mDatabase.SelectData(DatabaseContract.Purchases.TABLE, selectStatement, query);
			mTotalNumberOwned = (int)queryResult.Rows[0].Field<Int64>("TotalOwned");
		}

		/*
		 * Sets the total dividends received by accessing the Dividends database
		 */
		private void setTotalDividend()
		{
			// Gets the sum of the dividend payments from the dividends table
			String query = String.Format("WHERE {0} = '{1}'", DatabaseContract.Dividends.CODE, mCode);
			String selectStatement = String.Format("SUM({0}) AS TotalDividend", DatabaseContract.Dividends.AMOUNT);
			DataTable queryResult = mDatabase.SelectData(DatabaseContract.Dividends.TABLE, selectStatement, query);
			mTotalDividendValue = Convert.ToSingle(queryResult.Rows[0].Field<Int64?>("TotalDividend"))/100;
		}

		/*
		 * Sets the total amount spent by accessing the Purchases database
		 */
		private void setTotalSpent()
		{
			// Gets the sum of the number of shares bought from the purchases table
			String query = String.Format("WHERE {0} = '{1}'", DatabaseContract.Purchases.CODE, mCode);
			//String selectStatement = String.Format("SUM({0}) AS TotalSpent, SUM({1}) AS TotalOwned", DatabaseContract.Purchases.PRICE, DatabaseContract.Purchases.NUMBER);
			String selectStatement = String.Format("{0}, {1}", DatabaseContract.Purchases.PRICE, DatabaseContract.Purchases.NUMBER);
			DataTable queryResult = mDatabase.SelectData(DatabaseContract.Purchases.TABLE, selectStatement, query);
			// Loop over each row in the query result calculating the purchase cost and summing it to mTotalSpent
			mTotalSpent = 0;
			foreach(DataRow row in queryResult.Rows)
			{
				mTotalSpent+= Convert.ToSingle(row.Field<Int64>(DatabaseContract.Purchases.PRICE) * row.Field<Int64>(DatabaseContract.Purchases.NUMBER))/ 100;
			}
		}


		/* 
		 * Gets stock code
		 */
		public string getCode()
		{
			return mCode;
		}

		/* 
		 * Gets stock name
		 */
		public string getName()
		{
			return mName;
		}

		/* 
		 * Gets stocks current price
		 */
		public double getCurrentPrice()
		{
			return mCurrentPrice;
		}

		/* 
		 * Gets total number of shares owned
		 */
		public int getTotalNumberOwned()
		{
			return mTotalNumberOwned;
		}

		/* 
		 * Gets the total dividends received for this stock
		 */
		public double getTotalDividend()
		{
			return mTotalDividendValue;
		}

		/* 
		 * Gets the total spent buying this stock
		 */
		public double getTotalSpent()
		{
			return mTotalSpent;
		}

		/* 
		 * Gets the current value of this stock
		 */
		public double getTotalStockValue()
		{
			return getTotalNumberOwned() * getCurrentPrice();
		}

		/* 
		 * Gets the combined stock value and dividends recieved
		 */
		public double getTotalOverallValue()
		{
			return getTotalStockValue() + getTotalDividend();
		}

		/* 
		 * Gets the current profit in $
		 */
		public double getProfitDollar()
		{
			return getTotalOverallValue() - getTotalSpent();
		}

		/* 
		 * Gets the current profit as %
		 */
		public double getProfitPercent()
		{
			return getProfitDollar()/getTotalSpent() * 100;
		}

	}
}
