using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public abstract class AddEmployeeTemplate : UseCase
   {
      private readonly int empid;
      private readonly string name;
      private readonly string address;

      public AddEmployeeTemplate(int empid, string name, string address, PayrollDatabase database) : base (database)
      {
         this.empid = empid;
         this.name = name;
         this.address = address;
      }

      //Template pattern used to support the implementation of the MakeClassification and MakeSchedule methods
      //within the subclasses of AddEmployeeUseCase
      //All Employees need to make this objects but each employee type does it in a different way

      protected abstract PaymentClassification MakeClassification();
      protected abstract PaymentSchedule MakeSchedule();

      public override void Execute()
      {
         PaymentClassification pc = MakeClassification();
         PaymentSchedule ps = MakeSchedule();
         PaymentMethod pm = new HoldMethod();

         Employee e = new Employee(empid, name, address);
         e.Classification = pc;
         e.Schedule = ps;
         e.Method = pm;
         database.SaveEmployee(e);
         //TODO1 review how single entities are added to database with child records.  Only main entity is saved.  Seperate methods/classes are used to save/change/delete linked tables.
         //in memory employee may only have a subset of relational data tables loaded for specifc problem being worked on at the moment.  such as timecards for the month
         //review what happens if employee type is changed and therefore what is link to old timecards for emoployee moved to salaried
      }
   }
}
