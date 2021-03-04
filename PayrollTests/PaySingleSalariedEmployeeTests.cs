using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Tests
{
   [TestClass()]
   public class PaySingleEmployeeTests
   {
      private PayrollDatabase database = new InMemoryPayrollDatabase();
      [TestMethod()]
      public void PaySingleSalariedEmployee()
      {
         int empId = 1;
         AddSalariedEmployeeUseCase t = new AddSalariedEmployeeUseCase(empId, "Bob", "Home", 1000.00, database);
         t.Execute();
         DateTime payDate = new DateTime(2001, 11, 30);
         PaydayUseCase pt = new PaydayUseCase(payDate, database);
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
         AddSalariedEmployeeUseCase t = new AddSalariedEmployeeUseCase(
         empId, "Bob", "Home", 1000.00, database);
         t.Execute();
         DateTime payDate = new DateTime(2001, 11, 29);
         PaydayUseCase pt = new PaydayUseCase(payDate, database);
         pt.Execute();
         Paycheck pc = pt.GetPaycheck(empId);
         Assert.IsNull(pc);
      }
      [TestMethod()]
      public void PayingSingleHourlyEmployeeNoTimeCards()
      {
         int empId = 99;
         AddHourlyEmployeeUseCase t = new AddHourlyEmployeeUseCase(empId, "Bill", "Home", 15.25, database);
         t.Execute();
         DateTime payDate = new DateTime(2001, 11, 9);
         PaydayUseCase pt = new PaydayUseCase(payDate, database);
         pt.Execute();
         ValidateHourlyPaycheck(pt, empId, payDate, 0.0);
      }
      [TestMethod()]
      public void PaySingleHourlyEmployeeOneTimeCard()
      {
         int empId = 2;
         AddHourlyEmployeeUseCase t = new AddHourlyEmployeeUseCase(
         empId, "Bill", "Home", 15.25, database);
         t.Execute();
         DateTime payDate = new DateTime(2001, 11, 9); // Friday
         AddTimeCardUseCase tc = new AddTimeCardUseCase(payDate, 2.0, empId, database);
         tc.Execute();
         PaydayUseCase pt = new PaydayUseCase(payDate, database);
         pt.Execute();
         ValidateHourlyPaycheck(pt, empId, payDate, 30.5);
      }
      [TestMethod()]
      public void PaySingleHourlyEmployeeOvertimeOneTimeCard()
      {
         int empId = 2;
         AddHourlyEmployeeUseCase t = new AddHourlyEmployeeUseCase(
         empId, "Bill", "Home", 15.25, database);
         t.Execute();
         DateTime payDate = new DateTime(2001, 11, 9); // Friday
         AddTimeCardUseCase tc = new AddTimeCardUseCase(payDate, 9.0, empId, database);
         tc.Execute();
         PaydayUseCase pt = new PaydayUseCase(payDate, database);
         pt.Execute();
         ValidateHourlyPaycheck(pt, empId, payDate,
         (8 + 1.5) * 15.25);
      }

      [TestMethod()]
      public void PaySingleHourlyEmployeeTwoTimeCards()
      {
         int empId = 2;
         AddHourlyEmployeeUseCase t = new AddHourlyEmployeeUseCase(
         empId, "Bill", "Home", 15.25, database);
         t.Execute();
         DateTime payDate = new DateTime(2001, 11, 9); // Friday
         AddTimeCardUseCase tc = new AddTimeCardUseCase(payDate, 2.0, empId, database);
         tc.Execute();
         AddTimeCardUseCase tc2 = new AddTimeCardUseCase(payDate.AddDays(-1), 5.0, empId, database);
         tc2.Execute();
         PaydayUseCase pt = new PaydayUseCase(payDate, database);
         pt.Execute();
         ValidateHourlyPaycheck(pt, empId, payDate, 7 * 15.25);
      }

      [TestMethod()]
      public void PaySingleHourlyEmployeeOnWrongDate()
      {
         int empId = 2;
         AddHourlyEmployeeUseCase t = new AddHourlyEmployeeUseCase(
         empId, "Bill", "Home", 15.25, database);
         t.Execute();
         DateTime payDate = new DateTime(2001, 11, 8); // Thursday
         AddTimeCardUseCase tc = new AddTimeCardUseCase(payDate, 9.0, empId, database);
         tc.Execute();
         PaydayUseCase pt = new PaydayUseCase(payDate, database);
         pt.Execute();
         Paycheck pc = pt.GetPaycheck(empId);
         Assert.IsNull(pc);
      }
      [TestMethod()]
      public void TestPaySingleHourlyEmployeeWithTimeCardsSpanningTwoPayPeriods()
      {
         int empId = 2;
         AddHourlyEmployeeUseCase t = new AddHourlyEmployeeUseCase(
         empId, "Bill", "Home", 15.25, database);
         t.Execute();
         DateTime payDate = new DateTime(2001, 11, 9); // Friday
         DateTime dateInPreviousPayPeriod = new DateTime(2001, 11, 2);
         AddTimeCardUseCase tc = new AddTimeCardUseCase(payDate, 2.0, empId, database);
         tc.Execute();
         AddTimeCardUseCase tc2 = new AddTimeCardUseCase(
         dateInPreviousPayPeriod, 5.0, empId, database);
         tc2.Execute();
         PaydayUseCase pt = new PaydayUseCase(payDate, database);
         pt.Execute();
         ValidateHourlyPaycheck(pt, empId, payDate, 2 * 15.25);
      }
      [TestMethod()]
      public void SalariedUnionMemberDues()
      {
         int empId = 1;
         AddSalariedEmployeeUseCase t = new AddSalariedEmployeeUseCase(
         empId, "Bob", "Home", 1000.00, database);
         t.Execute();
         int memberId = 7734;
         ChangeAffiliationUseCase cmt =
         new ChangeAffiliationUseCase(empId, memberId, 9.42, database);
         cmt.Execute();
         DateTime payDate = new DateTime(2001, 11, 30);
         PaydayUseCase pt = new PaydayUseCase(payDate, database);
         pt.Execute();
         Paycheck pc = pt.GetPaycheck(empId);
         Assert.IsNotNull(pc);
         Assert.AreEqual(payDate, pc.PayDate);
         Assert.AreEqual(1000.0, pc.GrossPay, .001);
         Assert.AreEqual("Hold", pc.GetField("Disposition"));
         Assert.AreEqual(47.1, pc.Deductions, .001);
         Assert.AreEqual(1000.0 - 47.1, pc.NetPay, .001);
      }
      [TestMethod()]
      public void HourlyUnionMemberServiceCharge()
      {
         int empId = 1;
         AddHourlyEmployeeUseCase t = new AddHourlyEmployeeUseCase(
         empId, "Bill", "Home", 15.24, database);
         t.Execute();
         int memberId = 7734;
         ChangeAffiliationUseCase cmt = new ChangeAffiliationUseCase(empId, memberId, 9.42, database);
         cmt.Execute();
         DateTime payDate = new DateTime(2001, 11, 9);
         ServiceChargeUseCase sct = new ServiceChargeUseCase(memberId, payDate, 19.42, database);
         sct.Execute();
         AddTimeCardUseCase tct = new AddTimeCardUseCase(payDate, 8.0, empId, database);
         tct.Execute();
         PaydayUseCase pt = new PaydayUseCase(payDate, database);
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
         AddHourlyEmployeeUseCase t = new AddHourlyEmployeeUseCase(
         empId, "Bill", "Home", 15.24, database);
         t.Execute();
         int memberId = 7734;
         ChangeAffiliationUseCase cmt = new ChangeAffiliationUseCase(empId, memberId, 9.42, database);
         cmt.Execute();
         DateTime payDate = new DateTime(2001, 11, 9);
         DateTime earlyDate = new DateTime(2001, 11, 2); // previous Friday
         DateTime lateDate = new DateTime(2001, 11, 16); // next Friday
         ServiceChargeUseCase sct = new ServiceChargeUseCase(memberId, payDate, 19.42, database);
         sct.Execute();
         ServiceChargeUseCase sctEarly = new ServiceChargeUseCase(memberId, earlyDate, 100.00, database);
         sctEarly.Execute();
         ServiceChargeUseCase sctLate = new ServiceChargeUseCase(memberId, lateDate, 200.00, database);
         sctLate.Execute();
         AddTimeCardUseCase tct = new AddTimeCardUseCase(payDate, 8.0, empId, database);
         tct.Execute();
         PaydayUseCase pt = new PaydayUseCase(payDate, database);
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
      private void ValidateHourlyPaycheck(PaydayUseCase pt,int empid, DateTime payDate, double pay)
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
