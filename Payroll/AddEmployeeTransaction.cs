using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionNS
{
   abstract class AddEmployeeTransaction : Transaction
   {
      private int empID;
      private String itsAddress;
      private String itsName;
      public abstract void Execute();
   }
}
