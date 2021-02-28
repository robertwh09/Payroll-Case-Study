using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class ChangePrimaryFieldsUseCase : ChangeEmployeeUseCase
   {
      private readonly Employee employee;
      public ChangePrimaryFieldsUseCase(Employee employee, int id, IPayrollDatabase database) : base(id, database)
      {
         this.employee = employee;
      }
      protected override void Change(Employee e)
      {
         e.Name = employee.Name;
         e.Address = employee.Address;
         database.UpdateEmployee(e);
      }
   }
}
