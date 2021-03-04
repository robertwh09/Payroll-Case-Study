using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;

namespace Payroll.Tests
{
   [TestClass()]
   public class TimeCardUseCaseTests
   {
      private PayrollDatabase database;
      private int empId;

      [TestInitialize]
      public void TestInitialize()
      {
         this.database = new InMemoryPayrollDatabase();
         this.empId = 123;

         AddHourlyEmployeeUseCase t = new AddHourlyEmployeeUseCase(empId, "Bob", "Home1",10.5, database);
         t.Execute();
      }
      
      [TestMethod]
      public void AddTimecardUseCaseTest()
      {
         DateTime date = new DateTime(2021, 1, 1);
         double hours = 7.5;
         AddTimeCardUseCase atc = new AddTimeCardUseCase(date, hours, empId, database);
         atc.Execute();

         Employee employee = database.GetEmployee(empId);
         HourlyClassification hc = employee.Classification as HourlyClassification;

         Assert.AreEqual(hours, hc.GetTimeCard(date).Hours);
         Assert.AreEqual(date, hc.GetTimeCard(date).Date);

         hours = 8.5;
         atc = new AddTimeCardUseCase(date, hours, empId, database);
         atc.Execute();

         Assert.AreEqual(hours, hc.GetTimeCard(date).Hours);
         Assert.AreEqual(date, hc.GetTimeCard(date).Date);
      }
   }
}