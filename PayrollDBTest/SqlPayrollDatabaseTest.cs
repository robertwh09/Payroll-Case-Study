using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using System.Data;
using System;
using Payroll;

namespace PayrollMySQLDB.Tests
{
   [TestClass]
   public class SqlPayrollDatabaseTest
   {
      private IPayrollDatabase database;
      private MySqlConnection conn;
      private Employee employee;

      [TestInitialize]
      public void TestInitialize()
      {
         this.database = new MySqlPayrollDatabase();
         string connString = "Database=Payroll;Data Source=localhost;user id=sa;password=abc";
         conn = new MySql.Data.MySqlClient.MySqlConnection(connString); 
         conn.Open();

         ClearTables();

         employee = new Employee(123, "George", "123 Baker St.")
         {
            Schedule = new MonthlySchedule(),
            Method = new DirectDepositMethod("Bank 1", "123890"),
            Classification = new SalariedClassification(1000.00)
         };
      }

      private DataTable LoadTable(string table)
      {
         MySqlCommand command = new MySqlCommand("select * from " + table, conn);
         MySqlDataAdapter adapter = new MySqlDataAdapter(command);
         DataSet dataset = new DataSet();
         adapter.Fill(dataset);
         return dataset.Tables["table"];
      }

      [TestCleanup]
      public void TestCleanup()
      {
         conn.Close();
      }

      [TestMethod]
      public void AddEmployeeTest()
      {
         database.CreateEmployee(employee);

         MySqlDataAdapter adpt = new MySqlDataAdapter("select * from Employee", conn);
         DataSet dataset = new DataSet();
         adpt.Fill(dataset);
         DataTable table = dataset.Tables["table"];
         Assert.AreEqual(1, table.Rows.Count);
         DataRow row = table.Rows[0];
         Assert.AreEqual(123, row["EmpId"]); Assert.AreEqual("George", row["Name"]);
         Assert.AreEqual("123 Baker St.", row["Address"]);
         Assert.AreEqual("monthly", row["ScheduleType"]);
         ClearTables();
      }

      [TestMethod]
      public void ChangeEmployeePrimaryFieldsTest()
      {
         database.CreateEmployee(employee);
         Employee e = database.GetEmployee(123);
         e.Name = "Fred";
         e.Address = "256 Park Ln.";
         ChangePrimaryFieldsUseCase ec = new ChangePrimaryFieldsUseCase(e, 123, database);
         ec.Execute();

         e = database.GetEmployee(123);
         Assert.AreEqual("Fred", e.Name);
         Assert.AreEqual("256 Park Ln.", e.Address);

         ClearTables();
      }

      [TestMethod]
      public void AddEmployeeWhichAlreadyExistsTest()
      {
         const int MYSQL_DUPLICATE_PK = 1062;
         ClearTables();
         try
         {
            database.CreateEmployee(employee);
            database.CreateEmployee(employee);
            Assert.Fail("An duplicate PK exception needs to occur for this test to work.");
         }
         catch (MySqlException e)
         {
            Assert.AreEqual(MYSQL_DUPLICATE_PK, e.Number); //duplicate primary key 
         }
         ClearTables();
      }

      [TestMethod]
      public void ScheduleGetsSavedTest()
      {
         ClearTables();
         CheckSavedScheduleCode(new WeeklySchedule(), "weekly");
         ClearTables();
         CheckSavedScheduleCode(new MonthlySchedule(), "monthly");
         ClearTables();
         CheckSavedScheduleCode(new BiWeeklySchedule(), "biweekly");
      }

      private void CheckSavedScheduleCode(PaymentSchedule schedule, string expectedCode)
      {
         employee.Schedule = schedule;
         database.CreateEmployee(employee);
         DataTable table = LoadTable("employee");
         DataRow row = table.Rows[0];
         Assert.AreEqual(expectedCode, row["ScheduleType"]);
      }

      [TestMethod]
      public void PaymentMethodGetsSaved()
      {
         CheckSavedPaymentMethodCode(new HoldMethod(), "hold");
         ClearTables();
         CheckSavedPaymentMethodCode(new DirectDepositMethod("Bank -1", "0987654321"), "direct");
         ClearTables();
         CheckSavedPaymentMethodCode(new MailMethod("111 Maple Ct."), "mail");
      }

      [TestMethod]
      public void MailMethodGetsSaved()
      {
         CheckSavedPaymentMethodCode(new MailMethod("111 Maple Ct."), "mail");
         DataTable table = LoadTable("PaycheckAddress");
         Assert.AreEqual(1, table.Rows.Count);
         DataRow row = table.Rows[0];
         Assert.AreEqual("111 Maple Ct.", row["Address"]);
         Assert.AreEqual(123, row["EmpId"]);
      }

      private void CheckSavedPaymentMethodCode(PaymentMethod method, string expectedCode)
      {
         employee.Method = method;
         database.CreateEmployee(employee);
         DataTable table = LoadTable("Employee");
         DataRow row = table.Rows[0];
         Assert.AreEqual(expectedCode, row["PaymentMethodType"]);
      }

      [TestMethod]
      public void SaveIsTransactional()
      {
         ClearTables();
         // Null values won't go in the database.
         DirectDepositMethod method =  new DirectDepositMethod(null, null);
         employee.Method = method;
         try
         {
            database.CreateEmployee(employee);
            Assert.Fail("An exception needs to occur for this test to work.");
         }
         catch (MySqlException)
         { }
         DataTable table = LoadTable("Employee");
         Assert.AreEqual(0, table.Rows.Count);
      }

      [TestMethod]
      public void HourlyClassificationGetsSaved()
      {
         CheckSavedClassificationCode(new HourlyClassification(7.50), "hourly");

         DataTable table = LoadTable("HourlyClassification");

         Assert.AreEqual(1, table.Rows.Count);
         DataRow row = table.Rows[0];
         Assert.AreEqual(7.50, Convert.ToDouble(row["HourlyRate"]), 0.01);
         Assert.AreEqual(123, row["EmpId"]);
      }

      private void CheckSavedClassificationCode(PaymentClassification classification, string expectedCode)
      {
         employee.Classification = classification;
         database.CreateEmployee(employee);

         DataTable table = LoadTable("Employee");
         DataRow row = table.Rows[0];

         Assert.AreEqual(expectedCode, row["PaymentClassificationType"]);
      }

      [TestMethod]
      public void SalariedClassificationGetsSaved()
      {
         CheckSavedClassificationCode(new SalariedClassification(1234.56), "salary");

         DataTable table = LoadTable("SalariedClassification");

         Assert.AreEqual(1, table.Rows.Count);
         DataRow row = table.Rows[0];
         Assert.AreEqual(1234.56, Convert.ToDouble(row["Salary"]), 0.01);
         Assert.AreEqual(123, row["EmpId"]);
      }

      [TestMethod]
      public void CommissionClassificationGetsSaved()
      {
         CheckSavedClassificationCode(new CommissionedClassification(900.01, 15.5), "commission");

         DataTable table = LoadTable("CommissionedClassification");

         Assert.AreEqual(1, table.Rows.Count);
         DataRow row = table.Rows[0];
         Assert.AreEqual(900.01, Convert.ToDouble(row["Salary"]), 0.01);
         Assert.AreEqual(15.5, Convert.ToDouble(row["Commission"]), 0.01);
         Assert.AreEqual(123, row["EmpId"]);
      }

      [TestMethod]
      public void SaveMailMethodThenHoldMethod()
      {
         employee.Method = new MailMethod("123 Baker St.");
         database.CreateEmployee(employee);

         Employee employee2 = new Employee(321, "Ed", "456 Elm St.")
         {
            Method = new HoldMethod()
         };
         database.CreateEmployee(employee2);

         DataTable table = LoadTable("PaycheckAddress");
         Assert.AreEqual(1, table.Rows.Count);
      }

      [TestMethod]
      public void DirectDepositMethodGetsSaved()
      {
         CheckSavedPaymentMethodCode(new DirectDepositMethod("Bank -1", "0987654321"), "direct");

         DataTable table = LoadTable("DirectDepositAccount");

         Assert.AreEqual(1, table.Rows.Count);
         DataRow row = table.Rows[0];
         Assert.AreEqual("Bank -1", row["Bank"]);
         Assert.AreEqual("0987654321", row["Account"]);
         Assert.AreEqual(123, row["EmpId"]);
      }

      [TestMethod]
      public void LoadEmployee()
      {
         employee.Schedule = new BiWeeklySchedule();
         employee.Method = new DirectDepositMethod("1st Bank", "0123456");
         employee.Classification = new SalariedClassification(5432.10);
         database.CreateEmployee(employee);
         Employee loadedEmployee = database.GetEmployee(123);
         Assert.AreEqual(123, loadedEmployee.EmpId);
         Assert.AreEqual(employee.Name, loadedEmployee.Name);
         Assert.AreEqual(employee.Address, loadedEmployee.Address);
         PaymentSchedule schedule = loadedEmployee.Schedule;
         Assert.IsTrue(schedule is BiWeeklySchedule);
         PaymentMethod method = loadedEmployee.Method;
         Assert.IsTrue(method is DirectDepositMethod);
         DirectDepositMethod ddMethod = method as DirectDepositMethod;
         Assert.AreEqual("1st Bank", ddMethod.Bank);
         Assert.AreEqual("0123456", ddMethod.AccountNumber);
         PaymentClassification classification = loadedEmployee.Classification;
         Assert.IsTrue(classification is SalariedClassification);
         SalariedClassification salariedClassification = classification as SalariedClassification;
         Assert.AreEqual(5432.10, salariedClassification.Salary);
      }

      private void ClearTables()
      {
         ClearTable("SalariedClassification");
         ClearTable("CommissionedClassification");
         ClearTable("HourlyClassification");
         ClearTable("PaycheckAddress");
         ClearTable("DirectDepositAccount");
         ClearTable("Employee");
      }
      private void ClearTable(string tableName)
      {
         new MySqlCommand("delete from " + tableName, this.conn).ExecuteNonQuery();
      }
   }
}
