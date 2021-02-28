using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using Payroll;


namespace PayrollMySQLDB.Tests
{
   [TestClass()]
   public class SaveTimecardOperationTests
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

         //ClearTables();

         employee = new Employee(123, "George", "123 Baker St.")
         {
            Schedule = new MonthlySchedule(),
            Method = new DirectDepositMethod("Bank 1", "123890"),
            Classification = new HourlyClassification(10.50)
         };
      }

      [TestMethod]
      public void AddTimecardToEmployeeTest()
      {
         Timecard tc = new Timecard(new DateTime(2021, 1, 13), 4.5); //YYYY/MM/DD, hours
         HourlyClassification hc =  employee.Classification as HourlyClassification;
      }

      [TestMethod]
      public void AddTimecardToNonExistentEmployeeTest()
      {

      }

      [TestMethod]
      public void GetTimecardForNonExistentEmployeeTest()
      {

      }

      [TestMethod]
      public void GetAllTimecardsForEmployeeTest()
      {

      }

      [TestMethod]
      public void GetTimecardsForEmployeeDaterangeTest()
      {

      }
   }
}