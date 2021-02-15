using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll.Database;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;

namespace Payroll.Tests
{
   [TestClass()]
   public class SqlPayrollDatabaseTest
   {
      private IPayrollDatabase database;
      private MySqlConnection conn;
      private Employee employee;

      [TestInitialize()]
      public void SetUp()
      {
         this.database = new SqlPayrollDatabase();
         string connString = "Database=Payroll;Data Source=localhost;user id=sa;password=abc";
         conn = new MySql.Data.MySqlClient.MySqlConnection(connString); 
         conn.Open();
         ClearEmployeeTable();
         employee = new Employee(123, "George", "123 Baker St.");
         employee.Schedule = new MonthlySchedule();
         employee.Method = new DirectDepositMethod("Bank 1", "123890");
         employee.Classification = new SalariedClassification(1000.00);
      }
      private void ClearEmployeeTable()
      {
         new MySqlCommand("delete from Employee", this.conn).ExecuteNonQuery();
      }

      private DataTable LoadTable(string table)
      {
         MySqlCommand command = new MySqlCommand("select * from " + table, conn);
         MySqlDataAdapter adapter = new MySqlDataAdapter(command);
         DataSet dataset = new DataSet();
         adapter.Fill(dataset);
         return dataset.Tables["table"];
      }

      [TestCleanup()]
      public void TearDown()
      {
         conn.Close();
      }

      [TestMethod()]
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
         ClearEmployeeTable();
      }

      [TestMethod()]
      public void ScheduleGetsSavedTest()
      {
         ClearEmployeeTable();
         CheckSavedScheduleCode(new WeeklySchedule(), "weekly");
         ClearEmployeeTable();
         CheckSavedScheduleCode(new MonthlySchedule(), "monthly");
         ClearEmployeeTable();
         CheckSavedScheduleCode(new BiWeeklySchedule(), "bi-weekly");
      }
      private void CheckSavedScheduleCode(PaymentSchedule schedule, string expectedCode)
      {
         employee.Schedule = schedule;
         database.AddEmployee(employee);
         DataTable table = LoadTable("employee");
         DataRow row = table.Rows[0];
         Assert.AreEqual(expectedCode, row["ScheduleType"]);
      }

      [TestMethod()]
      public void PaymentMethodGetsSaved()
      {
         CheckSavedPaymentMethodCode(new HoldMethod(), "hold");
         ClearEmployeeTable();
         CheckSavedPaymentMethodCode(
         new DirectDepositMethod("Bank -1", "0987654321"), "direct");
         ClearEmployeeTable();
         CheckSavedPaymentMethodCode(new MailMethod("111 Maple Ct."), "mail");
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
   }
}
