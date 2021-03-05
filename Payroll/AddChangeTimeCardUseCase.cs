using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class AddChangeTimeCardUseCase : UseCase
   {
      private readonly DateTime date;
      private readonly double hours;
      private readonly int empId;
      public AddChangeTimeCardUseCase(int empId, DateTime date, double hours, PayrollDatabase database) : base (database)
      {
         this.date = date;
         this.hours = hours;
         this.empId = empId;
      }
      public override void Execute()
      {
         Employee e = database.GetEmployee(empId);
         if (e != null)
         {
            HourlyClassification hc = e.Classification as HourlyClassification;
            if (hc != null)
            {
               hc.AddChangeTimeCard(empId, date, hours, database);
            }
            else
               throw new InvalidOperationException("Tried to add timecard to non-hourly employee");
         }
         else
            throw new InvalidOperationException("No such employee.");
      }
   }
}
