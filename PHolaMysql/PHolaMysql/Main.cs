using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace Serpis.Ad
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			string connectionString = 
				"Server=localhost;" +
				"Database=dbprueba;" +
				"User Id=root;" +
				"Password=sistemas"; 
			
			MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
			
			mySqlConnection.Open ();
			
			//select * from categoria
			MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
			//mySqlCommand.CommandText = "select * from articulo where id=0";
			mySqlCommand.CommandText = "select * from articulo";
			MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader ();
			
			Console.WriteLine( string.Join("  ", getColumnNames(mySqlDataReader)) );
			
			//visualizar datos...
			while (mySqlDataReader.Read ()) {
				Console.WriteLine( getLine(mySqlDataReader) );
			}
			
			mySqlDataReader.Close();
			mySqlConnection.Close ();
			Console.WriteLine ("Ok"); 
		}
		
		private static string getLine(MySqlDataReader mySqlDataReader) {
			string line = "";
			for (int index = 0; index < mySqlDataReader.FieldCount; index++) {
				object value = mySqlDataReader.GetValue (index);
				if (value is DBNull)
					value = "null";
				line = line + value + "  ";
			}
			return line;
		}
		
		private static IEnumerable<string> getColumnNames(MySqlDataReader mySqlDataReader) {
			int fieldCount = mySqlDataReader.FieldCount;
			string[] columnNames = new string[ fieldCount ];
			for (int index = 0; index < fieldCount; index++)
				columnNames[index] = mySqlDataReader.GetName (index);
			return columnNames;
		}

		private static IEnumerable<string> getColumnNames2(MySqlDataReader mySqlDataReader) {
			int fieldCount = mySqlDataReader.FieldCount;
			List<string> columnNames = new List<string>();
			for (int index = 0; index < fieldCount; index++)
				columnNames.Add ( mySqlDataReader.GetName(index) );
			return columnNames;
		}
	}
}
