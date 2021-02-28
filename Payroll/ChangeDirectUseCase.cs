using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
	public class ChangeDirectUseCase : ChangeMethodUseCase
	{
		public ChangeDirectUseCase(int empId, IPayrollDatabase database) : base(empId, database)
		{
		}

		protected override PaymentMethod Method
		{
			get { return new DirectDepositMethod("Bank -1", "123"); }
		}

	}
}
