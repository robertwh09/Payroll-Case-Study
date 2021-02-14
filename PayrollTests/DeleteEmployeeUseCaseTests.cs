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
      private InMemoryPayrollDatabase database = new InMemoryPayrollDatabase();
      [TestMethod()]
      public void DeleteEmployeeUseCaseTest()
      {
         int empId = 4;
         int salary = 2500;
         double commissionRate = 3.2;
         AddCommissionedEmployeeUseCase t = new AddCommissionedEmployeeUseCase(empId, "Bill", "Home", salary, commissionRate, database);
         t.Execute();
         Employee e = database.GetEmployee(empId);
         Assert.IsNotNull(e);
         DeleteEmployeeUseCase dt =
         new DeleteEmployeeUseCase(empId, database);
         dt.Execute();
         e = database.GetEmployee(empId);
         Assert.IsNull(e);
      }
   }
}