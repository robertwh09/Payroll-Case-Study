using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public abstract class PaymentClassification
   {
      public abstract double CalculatePay(Paycheck paycheck);
   }
}
