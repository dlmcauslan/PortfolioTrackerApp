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
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTrackerApp
{
	public class DatabaseFunctions
	{
		//private String databasePath;
		private SQLiteConnection mDatabaseConnection;

		/**
		 * Constructor for Database Functions.
		 * Takes in as the input a string containing the path to the
		 * database file.
		 */
		public DatabaseFunctions(String databasePath)
		{
			//this.databasePath = databasePath;
			// Maybe run CreateDatabase here, then assign mDatabaseConnection
			//CreateDatabase();

			// This creates a database at the path given by databasePath if it does not exist
			// and creates the conneciton object.
			mDatabaseConnection = new SQLiteConnection("Datasource=" + databasePath);
		}

		/**
		 * Creates the database at the path given by databasePath
		 *  - if it does not exist
		 */
		//public void CreateDatabase()
		//{
		//	// Check whether the database exists - only if we need to
		//	Console.WriteLine(databasePath);
		//	// Creates the database at databasePath
		//	SQLiteConnection.CreateFile(databasePath);
		//}

		/** 
		 * Creates a table in the database using the provided createCommand string
		 */
		public void CreateTable(String createCommand)
		{
			// Open the connection then create the table using createCommand
			mDatabaseConnection.Open();
			SQLiteCommand command= new SQLiteCommand(createCommand, mDatabaseConnection);
			command.ExecuteNonQuery();
			mDatabaseConnection.Close();
		}

		/**
		 * Drops the given table
		 */
		public void DropTable(String tableName)
		{
			mDatabaseConnection.Open();
			String commandString = "DROP TABLE IF EXISTS " + tableName;
			SQLiteCommand command = new SQLiteCommand(commandString, mDatabaseConnection);
			command.ExecuteNonQuery();
			mDatabaseConnection.Close();
		}

		/**
		 * Inserts data into the given table where columns is a string containing the table
		 * columns separated by a comma, and values is a string containing the data values to 
		 * be added to the table separated by a comma.
		 * @return the number of rows that are modified by the INSERT command 
		 * - should be 1 if successful, 0 if unsuccessful
		 */
		public int InsertData(String tableName, String columns, String values)
		{
			try
			{
				mDatabaseConnection.Open();
				String commandString = "INSERT INTO " + tableName + " (" + columns
										+ ") values (" + values + ")";
				Console.WriteLine(commandString);
				SQLiteCommand command = new SQLiteCommand(commandString, mDatabaseConnection);	
				int numberRows = command.ExecuteNonQuery();
				mDatabaseConnection.Close();
				return numberRows;
			} catch
			{
				return 0;
			}
		}

		/**
		 * Deletes a particular row of data from the table based on the condition in
		 * the string 'condition'
		 * @return the number of rows that are deleted
		 */
		public int DeleteData(String tableName, String condition)
		{
			try
			{
				mDatabaseConnection.Open();
				String commandString = "DELETE FROM " + tableName + " WHERE " + condition;
				SQLiteCommand command = new SQLiteCommand(commandString, mDatabaseConnection);
				int numberRows = command.ExecuteNonQuery();
				mDatabaseConnection.Close();
				return numberRows;
			} catch
			{
				return 0;
			}
		}

		/**
		 * Edits the data of particular row of the table using the SET statement 'setStatement' and the 
		 * WHERE statement 'condition'
		 * @return 1 if the data was updated successfully, 0 if it wasn't
		 */
		public int EditData(String tableName, String setStatement, String condition)
		{
			try
			{
				mDatabaseConnection.Open();
				String commandString = "UPDATE " + tableName + " SET " + setStatement + 
										" WHERE " + condition;
				Console.WriteLine(commandString);
				SQLiteCommand command = new SQLiteCommand(commandString, mDatabaseConnection);
				int updateSuccess = command.ExecuteNonQuery();
				mDatabaseConnection.Close();
				return updateSuccess;
			} catch
			{
				return 0;
			}
		}

		/** 
		 * Performs a SELECT operation on the given table using given query and columns
		 * @return the SQLiteDataReader object that contains the query results
		 */
		public DataTable SelectData(String tableName, String columns="*", String query="")
		{
			mDatabaseConnection.Open();
			String commandString = "SELECT " + columns + " FROM " + tableName + " " + query;
			Console.WriteLine(commandString);
			SQLiteDataAdapter dataAdapter  = new SQLiteDataAdapter(commandString, mDatabaseConnection);
			DataTable data = new DataTable();
			dataAdapter.Fill(data);
			mDatabaseConnection.Close();
			return data;
		}
	}
}
