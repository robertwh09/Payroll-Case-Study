using MySql.Data.MySqlClient;
using Payroll;
using System;
using System.Data;

namespace PayrollMySQLDB
{
   public class LoadPaymentClassificationOperation
   {
		private readonly Employee employee;
		private readonly string classificationType;
		private readonly MySqlConnection connection;
		private string tableName;
		private delegate void ClassificationCreator(DataRow row);
		private ClassificationCreator classificationCreator;
		private PaymentClassification classification;

		public LoadPaymentClassificationOperation(Employee employee, string classificationType, MySqlConnection connection)
		{
			this.employee = employee;
			this.classificationType = classificationType;
			this.connection = connection;
		}

		public void Execute()
		{
			Prepare();
			DataRow row = LoadData();
			CreateClassification(row);
		}

		public void Prepare()
		{
			if (classificationType.Equals("hourly"))
			{
				tableName = "HourlyClassification";
				classificationCreator = new ClassificationCreator(CreateHourly);
			}
			else if (classificationType.Equals("salary"))
			{
				tableName = "SalariedClassification";
				classificationCreator = new ClassificationCreator(CreateSalaried);
			}
			else if (classificationType.Equals("commission"))
			{
				tableName = "CommissionedClassification";
				classificationCreator = new ClassificationCreator(CreateCommissioned);
			}
		}

		public MySqlCommand Command
		{
			get
			{
				string sql = String.Format("select * from {0} where EmpId=@EmpId", tableName);
				MySqlCommand command = new MySqlCommand(sql, connection);
				command.Parameters.AddWithValue("@EmpId", employee.EmpId);
				return command;
			}
		}

		private DataRow LoadData()
		{
			if (tableName != null)
				return LoadEmployeeOperation.LoadDataFromCommand(Command);
			else
				return null;
		}

		public void CreateClassification(DataRow row)
		{
			classificationCreator(row);
		}

		public PaymentClassification Classification
		{
			get { return classification; }
		}

		private void CreateHourly(DataRow row)
		{
			double rate = Convert.ToDouble(row["HourlyRate"]);
			classification = new HourlyClassification(rate);
		}

		private void CreateSalaried(DataRow row)
		{
			double salary = Convert.ToDouble(row["Salary"]);
			classification = new SalariedClassification(salary);
		}

		private void CreateCommissioned(DataRow row)
		{
			double baseRate = Convert.ToDouble(row["Salary"]);
			double commissionRate = Convert.ToDouble(row["Commission"]);
			classification = new CommissionedClassification(baseRate, commissionRate);
		}
	}
}