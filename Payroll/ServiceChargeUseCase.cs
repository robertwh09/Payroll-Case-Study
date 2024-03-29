﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class ServiceChargeUseCase : UseCase
   {
      private readonly int memberId;
      private readonly DateTime time;
      private readonly double charge;
      public ServiceChargeUseCase(int id, DateTime time, double charge, InMemoryPayrollDatabase database) : base (database)
      {
         this.memberId = id;
         this.time = time;
         this.charge = charge;
      }
      public override void Execute()
      {
         Employee e = database.GetUnionMember(memberId);
         if (e != null)
         {
            UnionAffiliation ua = null;
            if (e.Affiliation is UnionAffiliation)
               ua = e.Affiliation as UnionAffiliation;
            if (ua != null)
               ua.AddServiceCharge(
               new ServiceCharge(time, charge));
            else
               throw new InvalidOperationException(
               "Tries to add service charge to union"
               + "member without a union affiliation");
         }
         else
            throw new InvalidOperationException(
            "No such union member.");
      }
   }
}
