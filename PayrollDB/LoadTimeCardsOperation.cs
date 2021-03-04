using MySql.Data.MySqlClient;
using Payroll;
using System;
using System.Data;

namespace PayrollMySQLDB
{
   class LoadTimeCardsOperation
   {
		private readonly int empId;
		private readonly MySqlConnection connection;
		private HourlyClassification hourlyClassification;

		public LoadTimeCardsOperation(int empId, HourlyClassification hourlyClassification, MySqlConnection connection)
      {
			this.empId = empId;
			this.connection = connection;
			this.hourlyClassification = hourlyClassification;
      }
		public void Execute()
		{
			DataTable table = LoadDataTableFromCommand(Command);
         foreach (DataRow row in table.Rows)
         {
            DateTime date = (DateTime)row["Date"];
				double hours = (double)row["Hours"];
				TimeCard timecard = new TimeCard(date, hours);
				hourlyClassification.AddTimeCard(timecard);
			}
		}
		public MySqlCommand Command
		{
			get
			{
				string sql = String.Format("select * from TimeCard where EmpId=@EmpId");
				MySqlCommand command = new MySqlCommand(sql, connection);
				command.Parameters.AddWithValue("@EmpId", empId);
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
