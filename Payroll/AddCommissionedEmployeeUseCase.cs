using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class AddCommissionedEmployeeUseCase : AddEmployeeTemplate
   {
      private readonly double baseSalary;
      private readonly double commissionRate;
      public AddCommissionedEmployeeUseCase(int empid, string name, string address, double baseSalary, double commissionRate, PayrollDatabase database) 
         : base(empid, name, address, database)
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
