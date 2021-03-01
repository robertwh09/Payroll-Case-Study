using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll

{
   public abstract class UseCase
   {
      protected readonly PayrollDatabase database;

      public UseCase(PayrollDatabase database)
      {
         this.database = database;
      }
      public abstract void Execute();
   }
}
