using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class AddHourlyEmployeeUseCase : AddEmployeeUseCase
   {
      private double hourlyRate;
      public AddHourlyEmployeeUseCase(int empID, string name, string address, double hourlyRate) : base(empID, name, address)
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
