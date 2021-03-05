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
      public void AddChangeTimecardUseCaseTest()
      {
         DateTime date = new DateTime(2021, 1, 1);
         double hours = 7.5;
         AddChangeTimeCardUseCase atc = new AddChangeTimeCardUseCase(empId, date, hours, database);
         atc.Execute();

         Employee employee = database.GetEmployee(empId);
         HourlyClassification hc = employee.Classification as HourlyClassification;

         Assert.AreEqual(hours, hc.GetTimeCard(date).Hours);
         Assert.AreEqual(date, hc.GetTimeCard(date).Date);

         hours = 8.5;
         atc = new AddChangeTimeCardUseCase(empId, date, hours, database);
         atc.Execute();

         Assert.AreEqual(hours, hc.GetTimeCard(date).Hours);
         Assert.AreEqual(date, hc.GetTimeCard(date).Date);
      }

      [TestMethod]
      public void DeleteTimeCardTest ()
      {
         DateTime date = new DateTime(2021, 1, 1);
         double hours = 7.5;
         AddChangeTimeCardUseCase atc = new AddChangeTimeCardUseCase(empId, date, hours, database);
         atc.Execute();

         Employee employee = database.GetEmployee(empId);
         HourlyClassification hc = employee.Classification as HourlyClassification;

         Assert.AreEqual(hours, hc.GetTimeCard(date).Hours);
         Assert.AreEqual(date, hc.GetTimeCard(date).Date);

         DeleteTimeCardUseCase dtc = new DeleteTimeCardUseCase(empId, date, database);
         dtc.Execute();

         Assert.IsNull(hc.GetTimeCard(date));
      }

      [TestMethod]
      public void ChangeTimeCardTest()
      {

      }
   }
}