using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class AddTimecardUseCase : UseCase
   {
      private readonly int empId;
      private readonly Timecard timecard;

      public AddTimecardUseCase(int empId, Timecard timecard, PayrollDatabase database) : base (database)
      {
         this.empId = empId;
         this.timecard = timecard;
      }
      public override void Execute()
      {
         database.AddTimecard(empId, timecard);
      }
   }
}
