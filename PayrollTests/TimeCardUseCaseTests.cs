using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Tests
{
   [TestClass()]
   public class TimeCardUseCaseTests
   {
      [TestMethod()]
      public void TimeCardTransactionTest()
      {
         int empId = 5;
         double hourlyRate = 15.25;
         AddHourlyEmployeeUseCase t = new AddHourlyEmployeeUseCase(empId, "Bill", "Home", hourlyRate);
         t.Execute();
         TimeCardUseCase tct = new TimeCardUseCase(new DateTime(2005, 7, 31), 8.0, empId);
         tct.Execute();
         Employee e = PayrollDatabase.GetEmployee(empId);
         Assert.IsNotNull(e);
         PaymentClassification pc = e.Classification;
         Assert.IsTrue(pc is HourlyClassification);
         HourlyClassification hc = pc as HourlyClassification; TimeCard tc = hc.GetTimeCard(new DateTime(2005, 7, 31));
         Assert.IsNotNull(tc);
         Assert.AreEqual(8.0, tc.Hours);
      }
   }
}