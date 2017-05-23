/** DatabaseFunctions
 * 
 * A class that contains all of the functions that are used to create the
 * database and tables, then add, remove, or edit the database. As well as
 * remove the tables.
 * 
 * Created 23/05/2017 DLM
 * 
 */

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTrackerApp
{
	class DatabaseFunctions
	{
		private String databasePath;

		/**
		 * Constructor for Database Functions.
		 * Takes in as the input a string containing the path to the
		 * database file.
		 */
		public DatabaseFunctions(String databasePath)
		{
			this.databasePath = databasePath;
		}

		/**
		 * Creates the database at the path given by databasePath
		 *  - if it does not exist
		 */
		public void CreateDatabase()
		{
			// Check whether the database exists - only if we need to
			Console.WriteLine(databasePath);
			// Creates the database at databasePath
			SQLiteConnection.CreateFile(databasePath);
		}
	}
}
