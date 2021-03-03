using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using System.Data;
using System;
using Payroll;

namespace PayrollMySQLDB.Tests
{
	[TestClass]
	public class LoadPaymentMethodOperationTests
	{
		private Employee employee;
		private LoadPaymentMethodOperation operation;

		[TestInitialize]
		public void SetUp()
		{
			employee = new Employee(567, "Bill", "23 Pine Ct");
		}

		[TestMethod]
		public void LoadHoldMethod()
		{
			operation = new LoadPaymentMethodOperation(employee, "hold", null);
			operation.Execute();
			PaymentMethod method = this.operation.Method;
			Assert.IsTrue(method is HoldMethod);
		}

		[TestMethod]
		public void LoadDirectDepositMethodCommand()
		{
			operation = new LoadPaymentMethodOperation(employee, "direct", null);
			operation.Prepare();
			MySqlCommand command = operation.Command;
			Assert.AreEqual("select * from DirectDepositAccount where EmpId=@EmpId", command.CommandText);
			Assert.AreEqual(employee.EmpId, command.Parameters["@EmpId"].Value);
		}

		[TestMethod]
		public void CreateDirectDepositMethodFromRow()
		{
			operation = new LoadPaymentMethodOperation(employee, "direct", null);
			operation.Prepare();
			DataRow row = LoadEmployeeOperationTests.ShuntRow("Bank,Account", "1st Bank", "0123456");
			operation.CreatePaymentMethod(row);

			PaymentMethod method = this.operation.Method;
			Assert.IsTrue(method is DirectDepositMethod);
			DirectDepositMethod ddMethod = method as DirectDepositMethod;
			Assert.AreEqual("1st Bank", ddMethod.Bank);
			Assert.AreEqual("0123456", ddMethod.AccountNumber);
		}

		[TestMethod]
		public void LoadMailMethodCommand()
		{
			operation = new LoadPaymentMethodOperation(employee, "mail", null);
			operation.Prepare();
			MySqlCommand command = operation.Command;
			Assert.AreEqual("select * from PaycheckAddress where EmpId=@EmpId", command.CommandText);
			Assert.AreEqual(employee.EmpId, command.Parameters["@EmpId"].Value);
		}

		[TestMethod]
		public void CreateMailMethodFromRow()
		{
			operation = new LoadPaymentMethodOperation(employee, "mail", null);
			operation.Prepare();
			DataRow row = LoadEmployeeOperationTests.ShuntRow("Address", "23 Pine Ct");
			operation.CreatePaymentMethod(row);

			PaymentMethod method = this.operation.Method;
			Assert.IsTrue(method is MailMethod);
			MailMethod mailMethod = method as MailMethod;
			Assert.AreEqual("23 Pine Ct", mailMethod.Address);
		}
	}
}
