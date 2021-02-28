using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public abstract class AddEmployeeUseCase : UseCase
   {
      private readonly int empid;
      private readonly string name;
      private readonly string address;

      public AddEmployeeUseCase(int empid, string name, string address, IPayrollDatabase database) : base (database)
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
         database.CreateEmployee(e);
      }
   }
}
