using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Tests

{
   [TestClass()]
   public class PaySingleEmployeeTests
   {
      [TestMethod()]
      public void PaySingleSalariedEmployee()
      {
         int empId = 1;
         AddSalariedEmployee t = new AddSalariedEmployee(empId, "Bob", "Home", 1000.00);
         t.Execute();
         DateTime payDate = new DateTime(2001, 11, 30);
         PaydayTransaction pt = new PaydayTransaction(payDate);
         pt.Execute();
         Paycheck pc = pt.GetPaycheck(empId);
         Assert.IsNotNull(pc);
         Assert.AreEqual(payDate, pc.PayDate);
         Assert.AreEqual(1000.00, pc.GrossPay, .001);
         Assert.AreEqual("Hold", pc.GetField("Disposition"));
         Assert.AreEqual(0.0, pc.Deductions, .001);
         Assert.AreEqual(1000.00, pc.NetPay, .001);
      }

      [TestMethod()]
      public void PaySingleSalariedEmployeeOnWrongDate()
      {
         int empId = 1;
         AddSalariedEmployee t = new AddSalariedEmployee(
         empId, "Bob", "Home", 1000.00);
         t.Execute();
         DateTime payDate = new DateTime(2001, 11, 29);
         PaydayTransaction pt = new PaydayTransaction(payDate);
         pt.Execute();
         Paycheck pc = pt.GetPaycheck(empId);
         Assert.IsNull(pc);
      }
      [TestMethod()]
      public void PayingSingleHourlyEmployeeNoTimeCards()
      {
         int empId = 99;
         AddHourlyEmployee t = new AddHourlyEmployee(empId, "Bill", "Home", 15.25);
         t.Execute();
         DateTime payDate = new DateTime(2001, 11, 9);
         PaydayTransaction pt = new PaydayTransaction(payDate);
         pt.Execute();
         ValidateHourlyPaycheck(pt, empId, payDate, 0.0);
      }
      [TestMethod()]
      public void PaySingleHourlyEmployeeOneTimeCard()
      {
         int empId = 2;
         AddHourlyEmployee t = new AddHourlyEmployee(
         empId, "Bill", "Home", 15.25);
         t.Execute();
         DateTime payDate = new DateTime(2001, 11, 9); // Friday
         TimeCardTransaction tc = new TimeCardTransaction(payDate, 2.0, empId);
         tc.Execute();
         PaydayTransaction pt = new PaydayTransaction(payDate);
         pt.Execute();
         ValidateHourlyPaycheck(pt, empId, payDate, 30.5);
      }
      [TestMethod()]
      public void PaySingleHourlyEmployeeOvertimeOneTimeCard()
      {
         int empId = 2;
         AddHourlyEmployee t = new AddHourlyEmployee(
         empId, "Bill", "Home", 15.25);
         t.Execute();
         DateTime payDate = new DateTime(2001, 11, 9); // Friday
         TimeCardTransaction tc = new TimeCardTransaction(payDate, 9.0, empId);
         tc.Execute();
         PaydayTransaction pt = new PaydayTransaction(payDate);
         pt.Execute();
         ValidateHourlyPaycheck(pt, empId, payDate,
         (8 + 1.5) * 15.25);
      }

      [TestMethod()]
      public void PaySingleHourlyEmployeeTwoTimeCards()
      {
         int empId = 2;
         AddHourlyEmployee t = new AddHourlyEmployee(
         empId, "Bill", "Home", 15.25);
         t.Execute();
         DateTime payDate = new DateTime(2001, 11, 9); // Friday
         TimeCardTransaction tc = new TimeCardTransaction(payDate, 2.0, empId);
         tc.Execute();
         TimeCardTransaction tc2 = new TimeCardTransaction(payDate.AddDays(-1), 5.0, empId);
         tc2.Execute();
         PaydayTransaction pt = new PaydayTransaction(payDate);
         pt.Execute();
         ValidateHourlyPaycheck(pt, empId, payDate, 7 * 15.25);
      }

      [TestMethod()]
      public void PaySingleHourlyEmployeeOnWrongDate()
      {
         int empId = 2;
         AddHourlyEmployee t = new AddHourlyEmployee(
         empId, "Bill", "Home", 15.25);
         t.Execute();
         DateTime payDate = new DateTime(2001, 11, 8); // Thursday
         TimeCardTransaction tc = new TimeCardTransaction(payDate, 9.0, empId);
         tc.Execute();
         PaydayTransaction pt = new PaydayTransaction(payDate);
         pt.Execute();
         Paycheck pc = pt.GetPaycheck(empId);
         Assert.IsNull(pc);
      }
      [TestMethod()]
      public void TestPaySingleHourlyEmployeeWithTimeCardsSpanningTwoPayPeriods()
      {
         int empId = 2;
         AddHourlyEmployee t = new AddHourlyEmployee(
         empId, "Bill", "Home", 15.25);
         t.Execute();
         DateTime payDate = new DateTime(2001, 11, 9); // Friday
         DateTime dateInPreviousPayPeriod = new DateTime(2001, 11, 2);
         TimeCardTransaction tc = new TimeCardTransaction(payDate, 2.0, empId);
         tc.Execute();
         TimeCardTransaction tc2 = new TimeCardTransaction(
         dateInPreviousPayPeriod, 5.0, empId);
         tc2.Execute();
         PaydayTransaction pt = new PaydayTransaction(payDate);
         pt.Execute();
         ValidateHourlyPaycheck(pt, empId, payDate, 2 * 15.25);
      }
      [TestMethod()]
      public void HourlyUnionMemberServiceCharge()
      {
         int empId = 1;
         AddHourlyEmployee t = new AddHourlyEmployee(
         empId, "Bill", "Home", 15.24);
         t.Execute();
         int memberId = 7734;
         ChangeMemberTransaction cmt = new ChangeMemberTransaction(empId, memberId, 9.42);
         cmt.Execute();
         DateTime payDate = new DateTime(2001, 11, 9);
         ServiceChargeTransaction sct = new ServiceChargeTransaction(memberId, payDate, 19.42);
         sct.Execute();
         TimeCardTransaction tct = new TimeCardTransaction(payDate, 8.0, empId);
         tct.Execute();
         PaydayTransaction pt = new PaydayTransaction(payDate);
         pt.Execute();
         Paycheck pc = pt.GetPaycheck(empId);
         Assert.IsNotNull(pc);
         Assert.AreEqual(payDate, pc.PayPeriodEndDate);
         Assert.AreEqual(8 * 15.24, pc.GrossPay, .001);
         Assert.AreEqual("Hold", pc.GetField("Disposition"));
         Assert.AreEqual(9.42 + 19.42, pc.Deductions, .001);
         Assert.AreEqual((8 * 15.24) - (9.42 + 19.42), pc.NetPay, .001);
      }
      [TestMethod()]
      public void ServiceChargesSpanningMultiplePayPeriods()
      {
         int empId = 1;
         AddHourlyEmployee t = new AddHourlyEmployee(
         empId, "Bill", "Home", 15.24);
         t.Execute();
         int memberId = 7734;
         ChangeMemberTransaction cmt = new ChangeMemberTransaction(empId, memberId, 9.42);
         cmt.Execute();
         DateTime payDate = new DateTime(2001, 11, 9);
         DateTime earlyDate = new DateTime(2001, 11, 2); // previous Friday
         DateTime lateDate = new DateTime(2001, 11, 16); // next Friday
         ServiceChargeTransaction sct = new ServiceChargeTransaction(memberId, payDate, 19.42);
         sct.Execute();
         ServiceChargeTransaction sctEarly = new ServiceChargeTransaction(memberId, earlyDate, 100.00);
         sctEarly.Execute();
         ServiceChargeTransaction sctLate = new ServiceChargeTransaction(memberId, lateDate, 200.00);
         sctLate.Execute();
         TimeCardTransaction tct = new TimeCardTransaction(payDate, 8.0, empId);
         tct.Execute();
         PaydayTransaction pt = new PaydayTransaction(payDate);
         pt.Execute();
         Paycheck pc = pt.GetPaycheck(empId);
         Assert.IsNotNull(pc);
         Assert.AreEqual(payDate, pc.PayPeriodEndDate);
         Assert.AreEqual(8 * 15.24, pc.GrossPay, .001);
         Assert.AreEqual("Hold", pc.GetField("Disposition"));
         Assert.AreEqual(9.42 + 19.42, pc.Deductions, .001);
         Assert.AreEqual((8 * 15.24) - (9.42 + 19.42),
         pc.NetPay, .001);
      }
      private void ValidateHourlyPaycheck(PaydayTransaction pt,int empid, DateTime payDate, double pay)
      {
         Paycheck pc = pt.GetPaycheck(empid);
         Assert.IsNotNull(pc); Assert.AreEqual(payDate, pc.PayDate);
         Assert.AreEqual(pay, pc.GrossPay, .001);
         Assert.AreEqual("Hold", pc.GetField("Disposition"));
         Assert.AreEqual(0.0, pc.Deductions, .001);
         Assert.AreEqual(pay, pc.NetPay, .001);
      }
   }
}
