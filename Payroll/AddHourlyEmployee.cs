using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class AddHourlyEmployee : AddEmployeeTransaction
   {
      private double hourlyRate;
      public AddHourlyEmployee(int empID, string name, string address, double hourlyRate) : base(empID, name, address)
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
