using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public interface PaymentClassification
   {
      public double CalculatePay(Paycheck paycheck);
   }
}
