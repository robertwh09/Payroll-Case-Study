using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public interface PaymentMethod
   {
      public void Pay(Paycheck paycheck);
   }
}
