using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class AddCommissionedEmployee : AddEmployeeTransaction
   {
      private readonly double baseSalary;
      private readonly double commissionRate;
      public AddCommissionedEmployee(int empid, string name, string address, double baseSalary, double commissionRate) : base(empid, name, address)
      {
         this.baseSalary = baseSalary;
         this.commissionRate = commissionRate;
      }

      protected override PaymentClassification MakeClassification()
      {
         return new CommissionedClassification(baseSalary, commissionRate);
      }

      protected override PaymentSchedule MakeSchedule()
      {
         return new BiWeeklySchedule();
      }
   }
}
