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
			return (int)(100 * dollars);
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
	}
}
