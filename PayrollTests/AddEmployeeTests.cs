﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Tests
{
   [TestClass()]
   public class AddEmployeeTests
   {
      private InMemoryPayrollDatabase database = new InMemoryPayrollDatabase();
      [TestMethod()]
      public void AddSalariedEmployeeTest()
      {
         int empId = 1;
         double salary = 1000.0;
         AddSalariedEmployeeUseCase t = new AddSalariedEmployeeUseCase(empId, "Bob", "Home1", salary, database);
         t.Execute();
         Employee e = database.GetEmployee(empId);
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
      [TestMethod()]
      public void AddCommissionedEmployeeTest()
      {
         int empId = 2;
         double baseSalary = 950;
         double commissionRate = 13.5;

         AddCommissionedEmployeeUseCase t = new AddCommissionedEmployeeUseCase(empId, "Fred", "Home2", baseSalary, commissionRate, database);
         t.Execute();
         Employee e = database.GetEmployee(empId);
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
      [TestMethod()]
      public void AddHourlyEmployeeTest()
      {
         int empId = 3;
         double hourlyRate = 12.5;
         AddHourlyEmployeeUseCase t = new AddHourlyEmployeeUseCase(empId, "Freda", "Home3", hourlyRate, database);
         t.Execute();
         Employee e = database.GetEmployee(empId);
         Assert.AreEqual("Freda", e.Name);

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