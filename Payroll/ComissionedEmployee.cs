using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class ComissionedEmployee : Employee
   {
      public ComissionedEmployee(int empid, string name, string address) : base(empid, name, address)
      {
      }
   }
}
