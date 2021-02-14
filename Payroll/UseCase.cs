using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll

{
   public abstract class UseCase
   {
      protected readonly IPayrollDatabase database;

      public UseCase(IPayrollDatabase database)
      {
         this.database = database;
      }
      public abstract void Execute();
   }
}
