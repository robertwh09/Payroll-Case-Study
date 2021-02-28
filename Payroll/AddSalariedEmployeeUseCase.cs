using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class AddSalariedEmployeeUseCase : AddEmployeeUseCase
   {
      private readonly double salary;
      public AddSalariedEmployeeUseCase(int id, string name, string address, double salary, IPayrollDatabase database)
         : base(id, name, address, database)
      {
         this.salary = salary;
      }
      //Strategy pattern used to allocate correct pay calculation and pay schedule classifications to employee when it is contructed
      protected override PaymentClassification MakeClassification()
      {
         return new SalariedClassification(salary);
      }
      protected override PaymentSchedule MakeSchedule()
      {
         return new MonthlySchedule();
      }
   }
}
