﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class DirectMethod : PaymentMethod
   {
      void Pay(Paycheck paycheck)
      {
         throw new NotImplementedException();
      }

      void PaymentMethod.Pay(Paycheck paycheck)
      {
         throw new NotImplementedException();
      }
   }
}
