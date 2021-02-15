using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class WeeklySchedule : PaymentSchedule
   {
      public DateTime GetPayPeriodStartDate(DateTime date)
      {
         return date.AddDays(-6);
      }

      public bool IsPayDate(DateTime payDate)
      {
         return payDate.DayOfWeek == DayOfWeek.Friday;
      }

      public override string ToString()
      {
         return "weekly";
      }
   }
}
