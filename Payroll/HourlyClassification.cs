using System;
using System.Collections;
using System.Text;

namespace Payroll
{
   public class HourlyClassification : PaymentClassification
   {
      private double hourlyRate;
      private System.Collections.Hashtable timeCards = new Hashtable();

      public HourlyClassification(double hourlyRate)
      {
         this.hourlyRate = hourlyRate;
      }

      public double HourlyRate { get => hourlyRate; }

      public double CalculatePay(Paycheck paycheck)
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

      private double CalculatePayForTimeCard(TimeCard card)
      {
         double overtimeHours = Math.Max(0.0, card.Hours - 8);
         double normalHours = card.Hours - overtimeHours;
         return hourlyRate * normalHours + hourlyRate * 1.5 * overtimeHours;
      }

      internal void AddTimeCard(TimeCard timeCard)
      {
         timeCards[timeCard.Date] = timeCard;
      }

      public TimeCard GetTimeCard(DateTime dateTime)
      {
         return timeCards[dateTime.Date] as TimeCard;
      }
   }
}
