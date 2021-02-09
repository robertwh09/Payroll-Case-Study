using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Tests
{
   [TestClass()]
   public class ChangeEmployeeUseCaseTests
   {
      [TestMethod()]
      public void ChangeNameUseCaseTest()
      {
         int empId = 2;
         AddHourlyEmployeeUseCase t = new AddHourlyEmployeeUseCase(empId, "Bill", "Home", 15.25);
         t.Execute();

         ChangeNameUseCase cnt = new ChangeNameUseCase(empId, "Bob");
         cnt.Execute();

         Employee e = PayrollDatabase.GetEmployee(empId);
         Assert.IsNotNull(e);
         Assert.AreEqual("Bob", e.Name);
      }

      [TestMethod()]
      public void ChangeHourlyUseCaseTest()
      {
         int empId = 3;
         AddCommissionedEmployeeUseCase t =
         new AddCommissionedEmployeeUseCase(
         empId, "Lance", "Home", 2500, 3.2);
         t.Execute();
         ChangeHourlyUseCase cht =  new ChangeHourlyUseCase(empId, 27.52);
         cht.Execute();
         Employee e = PayrollDatabase.GetEmployee(empId);
         Assert.IsNotNull(e);
         PaymentClassification pc = e.Classification;
         Assert.IsNotNull(pc);
         Assert.IsTrue(pc is HourlyClassification);
         HourlyClassification hc = pc as HourlyClassification;
         Assert.AreEqual(27.52, hc.HourlyRate, .001);
         PaymentSchedule ps = e.Schedule;
         Assert.IsTrue(ps is WeeklySchedule);
      }

      [TestMethod()]
      public void ChangeUnionMember()
      {
         int empId = 8;
         AddHourlyEmployeeUseCase t = new AddHourlyEmployeeUseCase(empId, "Bill", "Home", 15.25);
         t.Execute();
         int memberId = 7743;
         double dues = 99.42;
         ChangeMemberUseCase cmt = new ChangeMemberUseCase(empId, memberId, dues);
         cmt.Execute();

         Employee e = PayrollDatabase.GetEmployee(empId);
         Assert.IsNotNull(e);
         Affiliation affiliation = e.Affiliation;
         Assert.IsNotNull(affiliation);
         Assert.IsTrue(affiliation is UnionAffiliation);

         UnionAffiliation uf = affiliation as UnionAffiliation;
         Assert.AreEqual(dues, uf.Dues, 0.001);
         Employee member = PayrollDatabase.GetUnionMember(memberId);
         Assert.IsNotNull(member);
         Assert.AreEqual(e, member);

         //Test removing Affiliation
         ChangeUnaffiliatedUseCase cmt1 = new ChangeUnaffiliatedUseCase(empId);
         cmt1.Execute();
         e = PayrollDatabase.GetEmployee(empId);
         Assert.IsNotNull(e);
         affiliation = e.Affiliation;
         Assert.IsNotNull(affiliation);
         Assert.IsTrue(affiliation is NoAffiliation);
      }
   }
}