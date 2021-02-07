using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Tests
{
   [TestClass()]
   public class AddSalariedEmployeeTests
   {
      [TestMethod()]
      public void AddSalariedEmployeeTest()
      {
         int empId = 1;
         double salary = 1000.0;
         AddSalariedEmployee t = new AddSalariedEmployee(empId, "Bob", "Home", salary);
         t.Execute();
         Employee e = PayrollDatabase.GetEmployee(empId);
         Assert.AreEqual("Bob", e.Name);
         PaymentClassification pc = e.Classification;
         Assert.IsTrue(pc is SalariedClassification);
         SalariedClassification sc = pc as SalariedClassification;
         Assert.AreEqual(salary, sc.Salary, .001);
         PaymentSchedule ps = e.Schedule;
         Assert.IsTrue(ps is MonthlySchedule);
         PaymentMethod pm = e.Method;
         Assert.IsTrue(pm is HoldMethod);
      }
   }
}