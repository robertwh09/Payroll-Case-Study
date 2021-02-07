using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class HoldMethod : PaymentMethod
   {
		public void Pay(Paycheck paycheck)
		{
			paycheck.SetField("Disposition", "Hold");
		}
   }
}
