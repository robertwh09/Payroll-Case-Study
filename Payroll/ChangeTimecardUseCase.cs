using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class ChangeTimecardUseCase : UseCase
   {
      private readonly Timecard timecard;
      private readonly int empId;
      public ChangeTimecardUseCase(int empId, Timecard timecard, PayrollDatabase database) : base(database)
      {
         this.timecard = timecard;
         this.empId = empId;
      }
      public override void Execute()
      {
         //TODO1 need to review how I have implemented timecard.  This is not implemented in other code base???
         throw new NotImplementedException();
      }
   }
}
