using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class ChangeAddressUseCase : ChangeEmployeeTemplate
   {
      private readonly string newAddress;
      public ChangeAddressUseCase(int empId, string newAddress, PayrollDatabase database) : base(empId, database)
      {
         this.newAddress = newAddress;
      }

      protected override void Change(Employee e)
      {
         e.Address = newAddress;
      }
   }
}
