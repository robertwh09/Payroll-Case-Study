using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Payroll.Tests
{
   [TestClass()]
   public class TimeCardUseCaseTests
   {
      private InMemoryPayrollDatabase database = new InMemoryPayrollDatabase();
      [TestMethod]
      public void AddTimecardTest()
      {
         int empId = 1;
         double salary = 1000.0;
         AddSalariedEmployeeUseCase t = new AddSalariedEmployeeUseCase(empId, "Bob", "Home1", salary, database);
         t.Execute();

         DateTime date = new DateTime(2021, 1, 3);
         Timecard tc = new Timecard(date, 10.5);
         AddTimecardUseCase tcu = new AddTimecardUseCase(empId, tc, database);
         tcu.Execute();

         Assert.AreEqual(tc, database.GetTimecard(empId, date));
      }
      [TestMethod]
      public void RemoveTimecardTest()
      {
      }
      [TestMethod]
      public void ChangeTimecardTest()
      {
      }

      [TestMethod]
      public void GetTimecardDateRangeTest()
      {
      }
   }
}