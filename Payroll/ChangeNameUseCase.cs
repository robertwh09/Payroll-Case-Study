using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class ChangeNameUseCase : ChangeEmployeeUseCase
   {
      private readonly string newName;
      public ChangeNameUseCase(int id, string newName, InMemoryPayrollDatabase database) : base(id, database)
      {
         this.newName = newName;
      }
      protected override void Change(Employee e)
      {
         e.Name = newName;
      }
   }
}
