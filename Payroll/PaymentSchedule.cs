using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public interface PaymentSchedule
   {
      DateTime GetPayPeriodStartDate(DateTime date);
      bool IsPayDate(DateTime date);
   }
}
