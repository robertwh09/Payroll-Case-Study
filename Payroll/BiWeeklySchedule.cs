using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class BiWeeklySchedule : PaymentSchedule
   {
      public DateTime GetPayPeriodStartDate(DateTime date)
      {
         return date.AddDays(-13);
      }

      public bool IsPayDate(DateTime payDate)
      {
         return payDate.DayOfWeek == DayOfWeek.Friday && payDate.Day % 2 == 0;
      }
      public override string ToString()
      {
         return "bi-weekly";
      }
   }
}
