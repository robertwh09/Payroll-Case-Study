using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class CommissionedClassification : PaymentClassification
   {
      private double commissionRate;
      private double baseSalary;
      private Hashtable salesReceipts = new Hashtable();

      public CommissionedClassification(double salary, double commissionRate)
      {
         this.baseSalary = salary;
         this.commissionRate = commissionRate;
      }

      public double CommissionRate { get => commissionRate; }
      public double BaseSalary { get => baseSalary; }

      public void AddSalesReceipt(SalesReceipt receipt)
      {
         salesReceipts[receipt.Date] = receipt;
      }

      public SalesReceipt GetSalesReceipt(DateTime time)
      {
         return salesReceipts[time] as SalesReceipt;
      }

      double PaymentClassification.CalculatePay(Paycheck paycheck)
      {
         throw new NotImplementedException();
      }
   }
}
