using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using System.Data;
using System;
using Payroll;

namespace PayrollMySQLDB.Tests
{
	[TestClass]
	public class LoadPaymentClassificationOperationTest
	{
		private Employee employee;
		private LoadPaymentClassificationOperation operation;

		[TestInitialize]
		public void SetUp()
		{
			employee = new Employee(567, "Bill", "23 Pine Ct");
		}

		[TestMethod]
		public void LoadHourlyCommand()
		{
			operation = new LoadPaymentClassificationOperation(employee, "hourly", null);
			operation.Prepare();
			MySqlCommand command = operation.Command;
			Assert.AreEqual("select * from HourlyClassification where EmpId=@EmpId", command.CommandText);
			Assert.AreEqual(employee.EmpId, command.Parameters["@EmpId"].Value);
		}

		[TestMethod]
		public void CreateDirectDepositMethodFromRow()
		{
			operation = new LoadPaymentClassificationOperation(employee, "hourly", null);
			operation.Prepare();
			DataRow row = LoadEmployeeOperationTests.ShuntRow("HourlyRate", 15.45);
			operation.CreateClassification(row);

			PaymentClassification classification = this.operation.Classification;
			Assert.IsTrue(classification is HourlyClassification);
			HourlyClassification hourlyClassification = classification as HourlyClassification;
			Assert.AreEqual(15.45, hourlyClassification.HourlyRate, 0.01);
		}

		[TestMethod]
		public void LoadSalariedCommand()
		{
			operation = new LoadPaymentClassificationOperation(employee, "salary", null);
			operation.Prepare();
			MySqlCommand command = operation.Command;
			Assert.AreEqual("select * from SalariedClassification where EmpId=@EmpId", command.CommandText);
			Assert.AreEqual(employee.EmpId, command.Parameters["@EmpId"].Value);
		}

		[TestMethod]
		public void CreateSalariedClassificationFromRow()
		{
			operation = new LoadPaymentClassificationOperation(employee, "salary", null);
			operation.Prepare();
			DataRow row = LoadEmployeeOperationTests.ShuntRow("Salary", 2500.00);
			operation.CreateClassification(row);

			PaymentClassification classification = this.operation.Classification;
			Assert.IsTrue(classification is SalariedClassification);
			SalariedClassification salariedClassification = classification as SalariedClassification;
			Assert.AreEqual(2500.00, salariedClassification.Salary, 0.01);
		}

		[TestMethod]
		public void LoadCommissionCommand()
		{
			operation = new LoadPaymentClassificationOperation(employee, "commission", null);
			operation.Prepare();
			MySqlCommand command = operation.Command;
			Assert.AreEqual("select * from CommissionedClassification where EmpId=@EmpId", command.CommandText);
			Assert.AreEqual(employee.EmpId, command.Parameters["@EmpId"].Value);
		}

		[TestMethod]
		public void CreateCommisstionedClassificationFromRow()
		{
			operation = new LoadPaymentClassificationOperation(employee, "commission", null);
			operation.Prepare();
			DataRow row = LoadEmployeeOperationTests.ShuntRow("Salary,Commission", 999.99, 9.9);
			operation.CreateClassification(row);

			PaymentClassification classification = this.operation.Classification;
			Assert.IsTrue(classification is CommissionedClassification);
			CommissionedClassification commissionClassification = classification as CommissionedClassification;
			Assert.AreEqual(999.99, commissionClassification.Salary, 0.01);
			Assert.AreEqual(9.9, commissionClassification.CommissionRate, 0.01);
		}
	}
}
