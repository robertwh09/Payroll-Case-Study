using MySql.Data.MySqlClient;
using Payroll;
using System;
using System.Data;

namespace PayrollMySQLDB
{
	class LoadTimeCardOperation
	{
		private readonly int empId;
		private readonly DateTime date;
		private readonly MySqlConnection connection;
		private HourlyClassification hourlyClassification;

		public LoadTimeCardOperation(int empId, DateTime date, MySqlConnection connection)
		{
			this.empId = empId;
			this.date = date;
			this.connection = connection;
			//this.hourlyClassification = hourlyClassification;
		}
		public TimeCard Execute()
		{
			DataTable table = LoadDataTableFromCommand(Command);
			if (table.Rows.Count == 0)
         {
				return null;
         }
			DataRow row = table.Rows[0];
			DateTime date = (DateTime)row["Date"];
			double hours = (double)row["Hours"];
			TimeCard timeCard = new TimeCard(date, hours);
			return timeCard;
		}
		public MySqlCommand Command
		{
			get
			{
				string sql = "SELECT Date, Hours FROM TimeCard WHERE (Date=@Date AND EmpId=@EmpID)";
				MySqlCommand command = new MySqlCommand(sql, connection);
				command.Parameters.AddWithValue("@EmpId", empId);
				command.Parameters.AddWithValue("@Date", date);
				return command;
			}
		}
		public static DataTable LoadDataTableFromCommand(MySqlCommand command)
		{
			MySqlDataReader dataReader = command.ExecuteReader();
			DataTable table = new DataTable();
			table.Load(dataReader);
			return table;
		}
	}
}

