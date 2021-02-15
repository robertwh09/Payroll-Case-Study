using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class BiWeeklySchedule : PaymentSchedule
   {
      DateTime PaymentSchedule.GetPayPeriodStartDate(DateTime date)
      {
         throw new NotImplementedException();
      }

      bool PaymentSchedule.IsPayDate(DateTime date)
      {
         throw new NotImplementedException();
      }
      public override string ToString()
      {
         return "bi-weekly";
      }
   }
}
