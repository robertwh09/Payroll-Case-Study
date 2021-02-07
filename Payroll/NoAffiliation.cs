using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class NoAffiliation : Affiliation
   {
      double Affiliation.CalculateDeductions(Paycheck paycheck)
      {
         return 0;
      }
   }
}
