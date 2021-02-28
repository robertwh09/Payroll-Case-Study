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
      private InMemoryPayrollDatabase database = new InMemoryPayrollDatabase();
      [TestMethod()]
      public void ChangeNameUseCaseTest()
      {
         int empId = 2;
         AddHourlyEmployeeUseCase t = new AddHourlyEmployeeUseCase(empId, "Bill", "Home", 15.25, database);
         t.Execute();

         ChangeNameUseCase cnUc = new ChangeNameUseCase(empId, "Bob", database);
         cnUc.Execute();

         Employee e = database.GetEmployee(empId);
         Assert.IsNotNull(e);
         Assert.AreEqual("Bob", e.Name);
      }

      [TestMethod()]
      public void ChangeAddressUseCaseTest()
      {
         int empId = 2;
         AddHourlyEmployeeUseCase u = new AddHourlyEmployeeUseCase(empId, "Bill", "Home", 15.25, database);
         u.Execute();
         
         ChangeAddressUseCase ca = new ChangeAddressUseCase(empId, "Away", database);
         ca.Execute();

         Employee e = database.GetEmployee(empId);
         Assert.IsNotNull(e);
         Assert.AreEqual("Away", e.Address);
         
      }

      [TestMethod()]
      public void ChangeHourlyUseCaseTest()
      {
         int empId = 3;
         AddCommissionedEmployeeUseCase t =
         new AddCommissionedEmployeeUseCase(empId, "Lance", "Home", 2500, 3.2, database);
         t.Execute();
         ChangeHourlyUseCase cht =  new ChangeHourlyUseCase(empId, 27.52, database);
         cht.Execute();
         Employee e = database.GetEmployee(empId);
         Assert.IsNotNull(e);
         PaymentClassification pc = e.Classification;
         Assert.IsNotNull(pc);
         Assert.IsTrue(pc is HourlyClassification);
         HourlyClassification hc = pc as HourlyClassification;
         Assert.AreEqual(27.52, hc.HourlyRate, .001);
         PaymentSchedule ps = e.Schedule;
         Assert.IsTrue(ps is WeeklySchedule);
      }

      [TestMethod]
      public void TestChangeSalaryTransaction()
      {
         int empId = 4;
         AddCommissionedEmployeeUseCase t = new AddCommissionedEmployeeUseCase(empId, "Lance", "Home", 2500, 3.2, database);
         t.Execute();
         ChangeSalaryUseCase cst = new ChangeSalaryUseCase(empId, 3000.00, database);
         cst.Execute();
         Employee e = database.GetEmployee(empId);
         Assert.IsNotNull(e);
         PaymentClassification pc = e.Classification;
         Assert.IsNotNull(pc);
         Assert.IsTrue(pc is SalariedClassification);
         SalariedClassification sc = pc as SalariedClassification;
         Assert.AreEqual(3000.00, sc.Salary, .001);
         PaymentSchedule ps = e.Schedule;
         Assert.IsTrue(ps is MonthlySchedule);
      }

      [TestMethod()]
      public void ChangeUnionMember()
      {
         int empId = 8;
         AddHourlyEmployeeUseCase t = new AddHourlyEmployeeUseCase(empId, "Bill", "Home", 15.25, database);
         t.Execute();
         int memberId = 7743;
         double dues = 99.42;
         ChangeAffiliationUseCase cmt = new ChangeAffiliationUseCase(empId, memberId, dues, database);
         cmt.Execute();

         Employee e = database.GetEmployee(empId);
         Assert.IsNotNull(e);
         Affiliation affiliation = e.Affiliation;
         Assert.IsNotNull(affiliation);
         Assert.IsTrue(affiliation is UnionAffiliation);

         UnionAffiliation uf = affiliation as UnionAffiliation;
         Assert.AreEqual(dues, uf.Dues, 0.001);
         Employee member = database.GetAffiliateMember(memberId);
         Assert.IsNotNull(member);
         Assert.AreEqual(e, member);

         //Test removing Affiliation
         ChangeToUnaffiliatedUseCase cmt1 = new ChangeToUnaffiliatedUseCase(empId, database);
         cmt1.Execute();
         e = database.GetEmployee(empId);
         Assert.IsNotNull(e);
         affiliation = e.Affiliation;
         Assert.IsNotNull(affiliation);
         Assert.IsTrue(affiliation is NoAffiliation);
      }

      [TestMethod]
      public void TestChangeCommisionTransaction()
      {
         int empId = 5;
         AddSalariedEmployeeUseCase t = new AddSalariedEmployeeUseCase(empId, "Bob", "Home", 2500.00, database);
         t.Execute();
         ChangeCommissionedUseCase cht = new ChangeCommissionedUseCase(empId, 1250.00, 5.6, database);
         cht.Execute();
         Employee e = database.GetEmployee(empId);
         Assert.IsNotNull(e);
         PaymentClassification pc = e.Classification;
         Assert.IsNotNull(pc);
         Assert.IsTrue(pc is CommissionedClassification);
         CommissionedClassification cc = pc as CommissionedClassification;
         Assert.AreEqual(1250.00, cc.BaseSalary, .001);
         Assert.AreEqual(5.6, cc.CommissionRate, .001);
         PaymentSchedule ps = e.Schedule;
         Assert.IsTrue(ps is BiWeeklySchedule);
      }

      [TestMethod]
      public void ChangeDirectMethod()
      {
         int empId = 6;
         AddSalariedEmployeeUseCase t = new AddSalariedEmployeeUseCase(empId, "Mike", "Home", 3500.00, database);
         t.Execute();
         ChangeDirectUseCase cddt = new ChangeDirectUseCase(empId, database);
         cddt.Execute();
         Employee e = database.GetEmployee(empId);
         Assert.IsNotNull(e);
         PaymentMethod method = e.Method;
         Assert.IsNotNull(method);
         Assert.IsTrue(method is DirectDepositMethod);
      }

      [TestMethod]
      public void ChangeHoldMethod()
      {
         int empId = 7;
         AddSalariedEmployeeUseCase t = new AddSalariedEmployeeUseCase(empId, "Mike", "Home", 3500.00, database);
         t.Execute();
         new ChangeDirectUseCase(empId, database).Execute();
         ChangeHoldUseCase cht = new ChangeHoldUseCase(empId, database);
         cht.Execute();
         Employee e = database.GetEmployee(empId);
         Assert.IsNotNull(e);
         PaymentMethod method = e.Method;
         Assert.IsNotNull(method);
         Assert.IsTrue(method is HoldMethod);
      }

      [TestMethod]
      public void ChangeMailMethod()
      {
         int empId = 8;
         AddSalariedEmployeeUseCase t = new AddSalariedEmployeeUseCase(empId, "Mike", "Home", 3500.00, database);
         t.Execute();
         ChangeMailUseCase cmt = new ChangeMailUseCase(empId, database);
         cmt.Execute();
         Employee e = database.GetEmployee(empId);
         Assert.IsNotNull(e);
         PaymentMethod method = e.Method;
         Assert.IsNotNull(method);
         Assert.IsTrue(method is MailMethod);
      }

      [TestMethod]
      public void ChangeUnaffiliatedMember()
      {
         int empId = 10;
         AddHourlyEmployeeUseCase t = new AddHourlyEmployeeUseCase(empId, "Bill", "Home", 15.25, database);
         t.Execute();
         int memberId = 7743;
         new ChangeAffiliationUseCase(empId, memberId, 99.42, database).Execute();
         ChangeToUnaffiliatedUseCase cut = new ChangeToUnaffiliatedUseCase(empId, database);
         cut.Execute();
         Employee e = database.GetEmployee(empId);
         Assert.IsNotNull(e);
         Affiliation affiliation = e.Affiliation;
         Assert.IsNotNull(affiliation);
         Assert.IsTrue(affiliation is NoAffiliation);
         Employee member = database.GetAffiliateMember(memberId);
         Assert.IsNull(member);
      }
   }
}