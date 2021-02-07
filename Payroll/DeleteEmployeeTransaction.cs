using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class DeleteEmployeeTransaction
   {
      private readonly int id;
      public DeleteEmployeeTransaction(int id)
      {
         this.id = id;
      }
      public void Execute()
      {
         PayrollDatabase.DeleteEmployee(id);
      }
   }
}
