using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Tests
{
   [TestClass()]
   public class AddHourlyEmployeeTests
   {
      [TestMethod()]
      public void AddHourlyEmployeeTest()
      {
         int empId = 2;
         double hourlyRate = 12.5;
         AddHourlyEmployee t = new AddHourlyEmployee(empId, "Fred", "Home", hourlyRate);
         t.Execute();
         Employee e = PayrollDatabase.GetEmployee(empId);
         Assert.AreEqual("Fred", e.Name);

         PaymentClassification pc = e.Classification;
         Assert.IsTrue(pc is HourlyClassification);

         HourlyClassification hc = pc as HourlyClassification;
         Assert.AreEqual(hourlyRate, hc.HourlyRate, .001);

         PaymentSchedule ps = e.Schedule;
         Assert.IsTrue(ps is WeeklySchedule);

         PaymentMethod pm = e.Method;
         Assert.IsTrue(pm is HoldMethod);
      }
   }
}