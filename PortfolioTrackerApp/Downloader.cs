using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTrackerApp
{
	class Downloader
	{
		private String mStockCode;
		private String mStartDate;
		private DatabaseFunctions mDatabase;
		private static String URL_BASE = "https://www.alphavantage.co/query?apikey=CRTAS5C17NF9TN6T&function=TIME_SERIES_DAILY&interval=1day&outputsize=full&symbol=";

		/**
		 * Constructor for Downloader Class which sets start date as MIN_DATE and the end date as todays date.
		 * @param stockCode - Code of the stock to download
		 */
		public Downloader(DatabaseFunctions database, String stockCode)
		{
			mDatabase = database;
			mStockCode = stockCode.ToUpper();
			// Get the most recent date that is already in the historical data table for the particular stock and set that
			// as mStartDate
			String query = String.Format("WHERE {0} = '{1}' ORDER BY {2} DESC LIMIT 1 ", DatabaseContract.Historical.CODE, mStockCode, DatabaseContract.Historical.DATE);
			//String query = String.Format("WHERE {0} = '{1}'", DatabaseContract.Historical.CODE, mStockCode);
			Console.WriteLine(query);
			DataTable queryResult = mDatabase.SelectData(DatabaseContract.Historical.TABLE, DatabaseContract.Historical.DATE, query);
			mStartDate = queryResult.Rows[0][DatabaseContract.Historical.DATE].ToString();
			Console.WriteLine(mStartDate);
		}

		/**
		 * download method. Downloads data for the stock between start and end date. Because the yahoo query language
		 * api has a maximum number of results per query, we download 1 year worth of data at a time in a loop.
		 * @throws IOException 
		 */
		public void download()
		{
			String historicalDataUrl = URL_BASE + mStockCode;
			Console.WriteLine(historicalDataUrl);
			using (WebClient downloader = new WebClient())
			{
				var jsonStr = downloader.DownloadString(historicalDataUrl);
				JObject stockData = JObject.Parse(jsonStr);

				// Loop over historical data and add it to the database
				foreach (var jItem in (JObject)stockData["Time Series (Daily)"])
				{
					// Parse the Date and Closing price for each data point
					String dataDate = jItem.Key;
					int dataPrice = Utilities.DollarsToCents((float)jItem.Value["4. close"]);
					// If the date is less than or equal to the most recent date currently in the database
					// then break out of the loop
					if (Utilities.compareDate(dataDate, mStartDate) < 1) break;
					// Otherwise add it to the database
					String values = String.Format("'{0}', '{1}', {2}", mStockCode, dataDate, dataPrice);
					mDatabase.InsertData(DatabaseContract.Historical.TABLE, DatabaseContract.Historical.COLUMNS, values);
				}
			}
		}
	}
}
