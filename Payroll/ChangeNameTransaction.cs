using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class ChangeNameTransaction : ChangeEmployeeTransaction
   {
      private readonly string newName;
      public ChangeNameTransaction(int id, string newName) : base(id)
      {
         this.newName = newName;
      }
      protected override void Change(Employee e)
      {
         e.Name = newName;
      }
   }
}
