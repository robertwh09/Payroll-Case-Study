using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class SalariedEmployee : Employee
   {
      public SalariedEmployee(int empid, string name, string address) : base(empid, name, address)
      {
      }
   }
}
