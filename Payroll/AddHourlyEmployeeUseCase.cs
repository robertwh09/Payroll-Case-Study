using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class AddHourlyEmployeeUseCase : AddEmployeeTemplate
   {
      private double hourlyRate;
      public AddHourlyEmployeeUseCase(int empID, string name, string address, double hourlyRate, PayrollDatabase database)
         : base(empID, name, address, database)
      {
         this.hourlyRate = hourlyRate;
      }

      protected override PaymentClassification MakeClassification()
      {
         return new HourlyClassification(hourlyRate);
      }

      protected override PaymentSchedule MakeSchedule()
      {
         return new WeeklySchedule();
      }
   }
}
