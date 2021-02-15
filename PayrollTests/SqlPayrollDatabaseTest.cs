using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Database;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;

namespace Payroll.Tests
{
   [TestClass]
   public class SqlPayrollDatabaseTest
   {
      private IPayrollDatabase database;
      private MySqlConnection conn;
      private Employee employee;

      [TestInitialize]
      public void SetUp()
      {
         this.database = new SqlPayrollDatabase();
         string connString = "Database=Payroll;Data Source=localhost;user id=sa;password=abc";
         conn = new MySql.Data.MySqlClient.MySqlConnection(connString); 
         conn.Open();
         ClearTables();
         employee = new Employee(123, "George", "123 Baker St.");
         employee.Schedule = new MonthlySchedule();
         employee.Method = new DirectDepositMethod("Bank 1", "123890");
         employee.Classification = new SalariedClassification(1000.00);
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
      public void TearDown()
      {
         conn.Close();
      }

      [TestMethod]
      public void AddEmployeeTest()
      {
         database.AddEmployee(employee);

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
         database.AddEmployee(employee);
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

      private void CheckSavedPaymentMethodCode(
      PaymentMethod method, string expectedCode)
      {
         employee.Method = method;
         database.AddEmployee(employee);
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
            database.AddEmployee(employee);
            Assert.Fail("An exception needs to occur for this test to work.");
         }
         catch (MySqlException)
         { }
         DataTable table = LoadTable("Employee");
         Assert.AreEqual(0, table.Rows.Count);
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
