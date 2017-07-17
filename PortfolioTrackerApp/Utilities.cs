using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* Utilities to class that contains a few user defined useful funtions */
namespace PortfolioTrackerApp
{
	static class Utilities
	{
		/*
		 *  Converts a string input in dollars, to an integer number of cents
		 */
		public static int DollarsToCents(float dollars)
		{
			return (int)Math.Round(100 * dollars);
		}

		/*
		 *  Takes as an input a string and returns true if it 
		 *  is a valid date in the format dd/mm/yyyy. Otherwise
		 *  returns false.
		 */
		public static bool checkDate(String date)
		{
			// Splits the string into [dd mm yyyy]
			String[] dateArray = date.Split('/');
			if (dateArray.Length != 3) return false;
			// Check that dd, mm, yyyy can all be parsed to ints
			if (!int.TryParse(dateArray[0], out int day)) return false;
			if (!int.TryParse(dateArray[1], out int month)) return false;
			if (!int.TryParse(dateArray[2], out int year)) return false;
			Console.WriteLine("Split all good");
			// Check that day, month, year are all valid
			try { DateTime dateObject = new DateTime(year, month, day); }
			catch { return false; };
			// Otherwise all tests have passed, so return true
			return true;
		}

		/* Takes as an input a date string in the format given by
		 * alphavantage.co (yyyy-mm-dd) and the database and converts it to a string
		 * of the format dd/mm/yyy
		 */
		public static string convertDate(String date)
		{
			// Splits the string into [yyyy mm dd]
			String[] dateArray = date.Split('-');
			String convertedDate = dateArray[2] + "/" + dateArray[1] + "/" + dateArray[0];
			return convertedDate;
		}

		/* Takes as an input a date string in the format given by
		 * dd/mm/yyy and converts it to a string
		 * of the format that is used in my database and alphavantage.co (yyyy-mm-dd)
		 */
		public static string convertDateReverse(String date)
		{
			// Splits the string into [dd mm yyyy]
			String[] dateArray = date.Split('/');
			String convertedDate = dateArray[2] + "-" + dateArray[1] + "-" + dateArray[0];
			return convertedDate;
		}

		/* Takes as an input two date Strings in the format yyyy-mm-dd.
		 * Returns 0 if the two dates are equal
		 * Returns 1 if date1 > date2
		 * Returns -1 if date1 < date2
		 */
		public static int compareDate(String date1, String date2)
		{
			// If theyre equal return 0
			if (date1.Equals(date2)) return 0;
			// Split the strings so we can compare day, month, year
			String[] dateArray1 = date1.Split('-'); // [yyyy, mm, dd]
			String[] dateArray2 = date2.Split('-');
			
			int[] dateInt1 = new int[3];
			int[] dateInt2 = new int[3];
			for (int i =0; i<3; i++)
			{
				//Console.WriteLine(dateArray1[i]);
				//Console.WriteLine(dateArray2[i]);
				dateInt1[i] = int.Parse(dateArray1[i]);
				dateInt2[i] = int.Parse(dateArray2[i]);
			}
			if (dateInt1[0] > dateInt2[0]) return 1;
			if (dateInt1[0] == dateInt2[0] && dateInt1[1] > dateInt2[1]) return 1;
			if (dateInt1[0] == dateInt2[0] && dateInt1[1] == dateInt2[1] && dateInt1[2] > dateInt2[2]) return 1;
			return -1;
		}

		/* Takes as an input a date string in the format given by
		* alphavantage.co (yyyy-mm-dd) and the database and converts it to a string
		* of the format dd/mm/yyy
		*/
		public static DateTime toDateTime(String date)
		{
			// Splits the string into [yyyy mm dd]
			String[] dateArray = date.Split('-');
			return new DateTime(int.Parse(dateArray[0]), int.Parse(dateArray[1]), int.Parse(dateArray[2]));
		}

	}

	

}
