using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Tests
{
   [TestClass()]
   public class SalesReceiptUseCaseTests
   {
      [TestMethod()]
      public void SalesReceiptTransactionTest()
      {
         int empId = 2;
         AddCommissionedEmployeeUseCase t = new AddCommissionedEmployeeUseCase(empId, "Bill", "Home", 1000, 13.5);
         t.Execute();

         Employee e = PayrollDatabase.GetEmployee(empId);
         Assert.IsNotNull(e);

         DateTime date = new DateTime(2005, 8, 8);
         double salesAmount = 505.25;
         SalesReceiptUseCase srt = new SalesReceiptUseCase(date, salesAmount, empId);
         srt.Execute();

         e = PayrollDatabase.GetEmployee(empId);
         CommissionedClassification cc = e.Classification as CommissionedClassification;
         SalesReceipt sr = cc.GetSalesReceipt(date);
         Assert.IsNotNull(sr);
         Assert.AreEqual(salesAmount, sr.SaleAmount, .001);
      }
   }
}