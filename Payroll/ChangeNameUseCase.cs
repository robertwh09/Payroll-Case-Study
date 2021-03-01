using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class ChangeNameUseCase : ChangeEmployeeTemplate
   {
      private readonly string newName;
      public ChangeNameUseCase(int empId, string newName, PayrollDatabase database) : base(empId, database)
      {
         this.newName = newName;
      }     
      protected override void Change(Employee e)
      {
         e.Name = newName;
      }
   }
}
