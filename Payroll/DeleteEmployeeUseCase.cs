using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class DeleteEmployeeUseCase : UseCase
   {
      private readonly int id;
      public DeleteEmployeeUseCase(int id, InMemoryPayrollDatabase database) : base (database)
      {
         this.id = id;
      }
      public override void Execute()
      {
         database.DeleteEmployee(id);
      }
   }
}
