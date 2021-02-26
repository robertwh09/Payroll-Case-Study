using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using System.Data;
using Payroll;

namespace PayrollDB.Tests
{
	[TestClass]
	public class LoadEmployeeOperationTest
	{
		private LoadEmployeeOperation operation;
		private Employee employee;

		[TestInitialize]
		public void SetUp()
		{
			employee = new Employee(123, "Jean", "10 Rue de Roi");
			operation = new LoadEmployeeOperation(123, null);

			operation.Employee = employee;
		}

		[TestMethod]
		public void LoadingEmployeeDataCommand()
		{
			operation = new LoadEmployeeOperation(123, null);
			MySqlCommand command = operation.LoadEmployeeCommand;
			Assert.AreEqual("select * from Employee where EmpId=@EmpId", command.CommandText);
			Assert.AreEqual(123, command.Parameters["@EmpId"].Value);
		}

		[TestMethod]
		public void LoadEmployeeData()
		{
			DataRow row = ShuntRow("Name,Address", "Jean", "10 Rue de Roi");

			operation.CreateEmplyee(row);

			Assert.IsNotNull(operation.Employee);
			Assert.AreEqual("Jean", operation.Employee.Name);
			Assert.AreEqual("10 Rue de Roi", operation.Employee.Address);
		}

		[TestMethod]
		public void LoadingSchedules()
		{
			DataRow row = ShuntRow("ScheduleType", "weekly");
			operation.AddSchedule(row);
			Assert.IsTrue(employee.Schedule is WeeklySchedule);

			row = ShuntRow("ScheduleType", "biweekly");
			operation.AddSchedule(row);
			Assert.IsTrue(employee.Schedule is BiWeeklySchedule);

			row = ShuntRow("ScheduleType", "monthly");
			operation.AddSchedule(row);
			Assert.IsTrue(employee.Schedule is MonthlySchedule);
		}

		[TestMethod]
		public void LoadingHoldMethod()
		{
			DataRow row = ShuntRow("PaymentMethodType", "hold");
			operation.AddPaymentMethod(row);
			Assert.IsTrue(employee.Method is HoldMethod);
		}

		public static DataRow ShuntRow(string columns, params object[] values)
		{
			DataTable table = new DataTable();
			foreach (string columnName in columns.Split(','))
				table.Columns.Add(columnName);
			return table.Rows.Add(values);
		}
	}
}
