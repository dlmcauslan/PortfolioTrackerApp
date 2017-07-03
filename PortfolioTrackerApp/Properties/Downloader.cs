using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTrackerApp.Properties
{
	class Downloader
	{
		private String mStockCode;
		private String mStartDate;
		private String mEndDate;
		private DatabaseFunctions mDatabase;
		private static String URL_BASE = "https://www.alphavantage.co/query?apikey=CRTAS5C17NF9TN6T&function=TIME_SERIES_DAILY&interval=1day&outputsize=full&symbol=";
    
		///**
		// * Constructor for Downloader Class
		// * @param stockCode - Code of the stock to download
		// * @param startDate - string for the start date of range of dates to be downloaded in format yyyy-mm-dd.
		// * @param endDate -  string for the end date of range of dates to be downloaded in format yyyy-mm-dd.
		// */
		//public Downloader(Database database, String stockCode, String startDate, String endDate)
		//	{
		//		this.database = database;
		//		this.stockCode = stockCode;
		//		this.startDate = startDate;
		//		this.endDate = endDate;
		//	}

		//	/**
		//	 * Constructor for Downloader Class which sets the end date as todays date.
		//	 * @param stockCode - Code of the stock to download
		//	 * @param startDate - string for the start date of range of dates to be downloaded in format yyyy-mm-dd.
		//	 */
		//	public Downloader(Database database, String stockCode, String startDate)
		//	{
		//		this.database = database;
		//		this.stockCode = stockCode;
		//		this.startDate = startDate;
		//		LocalDate localDate = LocalDate.now();
		//		this.endDate = DateTimeFormatter.ofPattern("yyyy-MM-dd").format(localDate);
		//	}

			/**
			 * Constructor for Downloader Class which sets start date as MIN_DATE and the end date as todays date.
			 * @param stockCode - Code of the stock to download
			 */
			public Downloader(DatabaseFunctions database, String stockCode)
			{
				mDatabase = database;
				mStockCode = stockCode;
				//mStartDate = MIN_DATE;
				//LocalDate localDate = LocalDate.now();
				//mEndDate = DateTimeFormatter.ofPattern("yyyy-MM-dd").format(localDate);
			}

			///**
			// * download method. Downloads data for the stock between start and end date. Because the yahoo query language
			// * api has a maximum number of results per query, we download 1 year worth of data at a time in a loop.
			// * @throws IOException 
			// */
			//public void download()
			//{
			//	// Get the range of years to loop over
			//	int minYear = Integer.parseInt(startDate.substring(0, 4));
			//	int maxYear = Integer.parseInt(endDate.substring(0, 4));
			//	String minMonthDay;
			//	String maxMonthDay;
        
			//	for (int y = minYear; y <= maxYear; y++) {
			//		// If its the first or last year, have a different start(end) date
			//		if (y == minYear) minMonthDay = startDate.substring(4);
			//		else minMonthDay = "-01-01";
			//		if (y == maxYear) maxMonthDay = endDate.substring(4);
			//		else maxMonthDay = "-12-31";
			//		// Create the minimum and maximum date string for the query
			//		String minDate = String.valueOf(y) + minMonthDay;
			//		String maxDate = String.valueOf(y) + maxMonthDay;
			//		// Create the query string and URL to download from
			//		String query = "where%20symbol%20%3D%20%22" + stockCode + "%22%20and%20startDate%20%3D%20%22" + minDate
			//				+ "%22%20and%20endDate%20%3D%20%22" + maxDate + "%22";
			//		URL DL_URL = new URL(URL_BASE + query + URL_END);
			//		System.out.println(DL_URL);
			//		//getDataFromURL(DL_URL);
			//		//            System.out.println(minDate);
			//		//            System.out.println(maxDate);
			//		System.out.println(DL_URL);
			//	}
			//}

		//    /**
		//     * Gets stock data from the URL supplied. Downloads json file and parses it to get date
		//     * and stock close price data.
		//     * @param DL_URL - URL to download the stock data from
		//     * @throws IOException
		//     */
		//    private void getDataFromURL(URL DL_URL) throws IOException {
		//        try(InputStream is = DL_URL.openStream();
		//                JsonReader rdr = Json.createReader(is)) {
		//            JsonObject obj = rdr.readObject();
		//            JsonArray dataArray = obj.getJsonObject("query").getJsonObject("results").getJsonArray("quote");
		//            // Loop over Json Array getting date and closing price data
		//            for (int i = dataArray.size() - 1; i >= 0; i--) {
		//                JsonObject dataObj = dataArray.getJsonObject(i);
		//                String date = dataObj.getString("Date");
		////                double close = Double.parseDouble(dataObj.getString("Close"));
		//                int close = Utilities.moneyStringToInt(dataObj.getString("Close"));
		////                System.out.println(date + " - $" + close);
		//                // TODO: Save in database.
		////                String values = "'VAS.AX', '2013-12-02', 70.21";
		//                String values = "'" + stockCode + "', '" + date + "', " + close;
		//                database.insertIntoTable(StockContract.Historical.TABLE_NAME, 
		//                                        StockContract.Historical.COLUMNS, values);
		//            }            
		//        }        
		//    }



		///**
		// * Getter for stock code
		// * @return string containing the stockCode
		// */
		//public String getStockCode()
		//{
		//	return stockCode;
		//}
	}
}
