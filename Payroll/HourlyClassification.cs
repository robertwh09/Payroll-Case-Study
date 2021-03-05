using System;
using System.Collections;
using System.Text;

namespace Payroll
{
   public class HourlyClassification : PaymentClassification
   {
      private double hourlyRate;
      private Hashtable timeCards = new Hashtable();

      public HourlyClassification(double hourlyRate)
      {
         this.hourlyRate = hourlyRate;
      }

      public double HourlyRate { get => hourlyRate; }

      public override double CalculatePay(Paycheck paycheck)
      {
         
         double totalPay = 0.0;
         foreach (TimeCard timeCard in timeCards.Values)
         {
            if (DateUtil.IsInPayPeriod(timeCard.Date,
               paycheck.PayPeriodStartDate,
               paycheck.PayPeriodEndDate))
               totalPay += CalculatePayForTimeCard(timeCard);
         }
         return totalPay;
      }

      private double CalculatePayForTimeCard(TimeCard timeCard)
      {
         double overtimeHours = Math.Max(0.0, timeCard.Hours - 8);
         double normalHours = timeCard.Hours - overtimeHours;
         return hourlyRate * normalHours + hourlyRate * 1.5 * overtimeHours;
      }

      public void AddChangeTimeCard(DateTime date, double hours)
      {
         TimeCard timeCard = new TimeCard(date, hours);
         timeCards[timeCard.Date] = timeCard;
      }
      public void AddChangeTimeCard(int empId, DateTime date, double hours, PayrollDatabase database)
      {
         database.AddTimeCard(empId, new TimeCard(date, hours));
         AddChangeTimeCard(date, hours);
      }

      public TimeCard GetTimeCard(DateTime date)
      {
         if (timeCards[date] == null) return null;
         return timeCards[date] as TimeCard;
      }

      private void  DeleteTimeCard(DateTime date)
      {
         timeCards.Remove(date);
      }

      public void DeleteTimeCard(int empId, DateTime date, PayrollDatabase database)
      {
         database.DeleteTimeCard(empId, date);
         DeleteTimeCard(date);
      }


      public Hashtable GetAllTimeCards()
      {
         return timeCards;
      }
      
      public override string ToString()
      {
         return String.Format("${0}/hr", hourlyRate);
      }
   }
}
