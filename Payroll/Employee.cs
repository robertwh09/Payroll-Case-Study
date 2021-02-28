using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class Employee
   {
      private readonly int empid;
      private string name;
      private string address;
      private PaymentClassification classification;
      private PaymentSchedule schedule;
      private PaymentMethod method;
      private Affiliation affiliation = new NoAffiliation();
      public Employee(int empid, string name, string address)
      {
         this.empid = empid;
         this.name = name;
         this.address = address;
      }

      public int EmpId
      {
         get { return empid; }
      }
      public string Name
      {
         get { return name; }
         set { name = value; }
      }
      public string Address
      {
         get { return address; }
         set { address = value; }
      }
      public PaymentClassification Classification
      {
         get { return classification; }
         set { classification = value; }
      }
      public PaymentSchedule Schedule
      {
         get { return schedule; }
         set { schedule = value; }
      }
      public PaymentMethod Method
      {
         get { return method; }
         set { method = value; }
      }
      public Affiliation Affiliation
      {
         get { return affiliation; }
         set { affiliation = value; }
      }

      public bool IsPayDate(DateTime date)
      {
         return schedule.IsPayDate(date);
      }
      public void Payday(Paycheck paycheck)
      {
         double grossPay = classification.CalculatePay(paycheck);
         double deductions = affiliation.CalculateDeductions(paycheck);
         double netPay = grossPay - deductions;
         paycheck.GrossPay = grossPay;
         paycheck.Deductions = deductions;
         paycheck.NetPay = netPay;
         method.Pay(paycheck);
      }
      public DateTime GetPayPeriodStartDate(DateTime date)
      {
         return schedule.GetPayPeriodStartDate(date);
      }
   }
}
