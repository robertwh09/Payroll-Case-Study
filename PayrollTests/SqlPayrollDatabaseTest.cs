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

      [TestInitialize()]
      public void SetUp()
      {
         this.database = new SqlPayrollDatabase();
         string connString = "Database=Payroll;Data Source=localhost;user id=sa;password=abc";
         conn = new MySql.Data.MySqlClient.MySqlConnection(connString); 
         conn.Open();
         MySqlCommand cmd = new MySqlCommand("delete from Employee", conn);
         cmd.ExecuteNonQuery();
         conn.Close();
      }

      [TestCleanup()]
      public void TearDown()
      {
         conn.Close();
      }

      [TestMethod()]
      public void AddEmployeeTest()
      {
         Employee employee = new Employee(123, "George", "123 Baker St.");
         employee.Schedule = new MonthlySchedule();
         employee.Method = new DirectDepositMethod("Bank 1", "123890");
         employee.Classification = new SalariedClassification(1000.00);
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
      }
   }
}
