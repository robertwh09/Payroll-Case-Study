using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   //Template Pattern used here.  All changes require the Employee to be retrieved
   //But the specific change needed is 'Change' by the subclass specific to the type of change
   public abstract class ChangeEmployeeTemplate : UseCase
   {
      private readonly int empId;
      public ChangeEmployeeTemplate(int empId, PayrollDatabase database) : base (database)
      {
         this.empId = empId;
      }
      public override void Execute()
      {
         Employee e = database.GetEmployee(empId);
         if (e != null)
            Change(e);
         else
            throw new InvalidOperationException("No such employee.");
      }
      protected abstract void Change(Employee e);
   }
}
