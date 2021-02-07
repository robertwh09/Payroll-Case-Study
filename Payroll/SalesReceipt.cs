using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class SalesReceipt
   {
      public SalesReceipt(DateTime date, double saleAmount)
      {
         Date = date;
         SaleAmount = saleAmount;
      }

      public DateTime Date { get; }
      public double SaleAmount { get; }
   }
}
