﻿using Payroll;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Tests
{
   [TestClass()]
   public class ServiceChargeUseCaseTests
   {
      private InMemoryPayrollDatabase database = new InMemoryPayrollDatabase();

      [TestMethod()]
      public void ServiceChargeUseCaseTest()
      {
         int empId = 2;
         AddHourlyEmployeeUseCase t = new AddHourlyEmployeeUseCase(empId, "Bill", "Home", 15.25, database);
         t.Execute();

         Employee e = database.GetEmployee(empId);
         Assert.IsNotNull(e);
         UnionAffiliation af = new UnionAffiliation();
         e.Affiliation = af;
         int memberId = 86; // Maxwell Smart
         double serviceCharge = 12.95;
         DateTime date = new DateTime(2005, 8, 8);

         database.AddUnionMember(memberId, e);
         ServiceChargeUseCase sct = new ServiceChargeUseCase(memberId, date, serviceCharge, database);
         sct.Execute();

         e = database.GetEmployee(empId);
         af = e.Affiliation as UnionAffiliation;
         ServiceCharge sc = af.GetServiceCharge(date);
         Assert.IsNotNull(sc);
         Assert.AreEqual(serviceCharge, sc.Amount, .001);
      }
   }
}