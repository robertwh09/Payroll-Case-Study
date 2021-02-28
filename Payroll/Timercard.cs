using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class Timecard
   {
      private readonly DateTime date;
      private readonly double hours;
      public Timecard(DateTime date, double hours)
      {
         this.date = date;
         this.hours = hours;
      }
      public double Hours
      {
         get { return hours; }
      }
      public DateTime Date
      {
         get { return date; }
      }
   }
}
