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

      private double CalculatePayForTimeCard(TimeCard card)
      {
         double overtimeHours = Math.Max(0.0, card.Hours - 8);
         double normalHours = card.Hours - overtimeHours;
         return hourlyRate * normalHours + hourlyRate * 1.5 * overtimeHours;
      }

      public void AddTimeCard(TimeCard timeCard)
      {
         //TODO save to database here
         timeCards[timeCard.Date] = timeCard;
      }

      public TimeCard GetTimeCard(DateTime dateTime)
      {
         //TODO retrieve from database here
         return timeCards[dateTime.Date] as TimeCard;
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
