using Payroll;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Tests
{
   [TestClass()]
   public class ServiceChargeTransactionTests
   {
      [TestMethod()]
      public void ServiceChargeTransactionTest()
      {
         int empId = 2;
         AddHourlyEmployee t = new AddHourlyEmployee(empId, "Bill", "Home", 15.25);
         t.Execute();

         Employee e = PayrollDatabase.GetEmployee(empId);
         Assert.IsNotNull(e);
         UnionAffiliation af = new UnionAffiliation();
         e.Affiliation = af;
         int memberId = 86; // Maxwell Smart
         double serviceCharge = 12.95;
         DateTime date = new DateTime(2005, 8, 8);

         PayrollDatabase.AddUnionMember(memberId, e);
         ServiceChargeTransaction sct = new ServiceChargeTransaction(memberId, date, serviceCharge);
         sct.Execute();

         e = PayrollDatabase.GetEmployee(empId);
         af = e.Affiliation as UnionAffiliation;
         ServiceCharge sc = af.GetServiceCharge(date);
         Assert.IsNotNull(sc);
         Assert.AreEqual(serviceCharge, sc.Amount, .001);
      }
   }
}