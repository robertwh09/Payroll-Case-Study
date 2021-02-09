using Microsoft.VisualStudio.TestTools.UnitTesting;
using Payroll;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Tests
{
   [TestClass()]
   public class DeleteEmployeeUseCaseTests
   {
      [TestMethod()]
      public void DeleteEmployeeTransactionTest()
      {
         int empId = 4;
         int salary = 2500;
         double commissionRate = 3.2;
         AddCommissionedEmployeeUseCase t = new AddCommissionedEmployeeUseCase(empId, "Bill", "Home", salary, commissionRate);
         t.Execute();
         Employee e = PayrollDatabase.GetEmployee(empId);
         Assert.IsNotNull(e);
         DeleteEmployeeUseCase dt =
         new DeleteEmployeeUseCase(empId);
         dt.Execute();
         e = PayrollDatabase.GetEmployee(empId);
         Assert.IsNull(e);
      }
   }
}