using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class SalariedClassification : PaymentClassification
   {
      private double salary;
      public SalariedClassification(double salary)
      {
         this.Salary = salary;
      }

      public double Salary { get => salary; set => salary = value; }

      public override double CalculatePay(Paycheck paycheck)
      {
         return salary;
      }
      public override string ToString()
      {
         return String.Format("${0}", salary);
      }
   }
}
