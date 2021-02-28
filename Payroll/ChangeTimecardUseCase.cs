using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class ChangeTimecardUseCase : UseCase
   {
      private readonly Timecard timecard;
      private readonly int empId;
      public ChangeTimecardUseCase(int empId, Timecard timecard, IPayrollDatabase database) : base(database)
      {
         this.timecard = timecard;
         this.empId = empId;
      }
      public override void Execute()
      {
         
         throw new NotImplementedException();
      }
   }
}
