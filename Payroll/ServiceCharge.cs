using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class ServiceCharge
   {
      private DateTime date;
      private double amount;

      public ServiceCharge(DateTime time, double charge)
      {
         this.date = time;
         this.amount = charge;
      }

      public DateTime Date { get => date; }
      public double Amount { get => amount; }
   }
}
