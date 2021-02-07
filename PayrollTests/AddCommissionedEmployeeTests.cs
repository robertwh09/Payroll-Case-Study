using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Tests
{
   [TestClass()]
   public class AddCommissionedEmployeeTests
   {
      [TestMethod()]
      public void AddCommissionedEmployeeTest()
      {
         int empId = 3;
         double baseSalary = 950;
         double commissionRate = 13.5;

         AddCommissionedEmployee t = new AddCommissionedEmployee(empId, "Fred", "Home", baseSalary, commissionRate);
         t.Execute();
         Employee e = PayrollDatabase.GetEmployee(empId);
         Assert.AreEqual("Fred", e.Name);

         PaymentClassification pc = e.Classification;
         Assert.IsTrue(pc is CommissionedClassification);

         CommissionedClassification cc = pc as CommissionedClassification;
         Assert.AreEqual(baseSalary, cc.BaseSalary, .001);

         CommissionedClassification hc = pc as CommissionedClassification;
         Assert.AreEqual(commissionRate, hc.CommissionRate, .001);

         PaymentSchedule ps = e.Schedule;
         Assert.IsTrue(ps is BiWeeklySchedule);

         PaymentMethod pm = e.Method;
         Assert.IsTrue(pm is HoldMethod);
      }
   }
}