using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class DeleteEmployeeUseCase
   {
      private readonly int id;
      public DeleteEmployeeUseCase(int id)
      {
         this.id = id;
      }
      public void Execute()
      {
         PayrollDatabase.DeleteEmployee(id);
      }
   }
}
