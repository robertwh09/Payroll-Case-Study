using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class DeleteTimeCardUseCase : UseCase
   {
      private readonly int empId;
      private readonly DateTime date;
      public DeleteTimeCardUseCase(int empId, DateTime date, PayrollDatabase database) : base(database)
      {
         this.empId = empId;
         this.date = date;
      }
      public override void Execute()
      {
         Employee e = database.GetEmployee(empId);
         if (e != null)
         {
            HourlyClassification hc = e.Classification as HourlyClassification;
            if (hc != null)
            {
               hc.DeleteTimeCard(e.EmpId, date, database);
            }
            else
               throw new InvalidOperationException("Tried to delete timecard for non-hourly employee");
         }
         else
            throw new InvalidOperationException("No such employee.");
      }
   }
}
