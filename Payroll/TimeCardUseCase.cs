﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class TimeCardUseCase : UseCase
   {
      private readonly DateTime date;
      private readonly double hours;
      private readonly int empId;
      public TimeCardUseCase(DateTime date, double hours, int empId, InMemoryPayrollDatabase database) : base (database)
      {
         this.date = date;
         this.hours = hours;
         this.empId = empId;
      }
      public override void Execute()
      {
         Employee e = database.GetEmployee(empId);
         if (e != null)
         {
            HourlyClassification hc = e.Classification as HourlyClassification;
            if (hc != null)
               hc.AddTimeCard(new TimeCard(date, hours));
            else
               throw new InvalidOperationException(
               "Tried to add timecard to" +
               "non-hourly employee");
         }
         else
            throw new InvalidOperationException("No such employee.");
      }
   }
}
