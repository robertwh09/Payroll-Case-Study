using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
	public class ChangeHoldUseCase : ChangeMethodUseCase
	{
		public ChangeHoldUseCase(int empId, PayrollDatabase database)
			: base(empId, database)
		{
		}

		protected override PaymentMethod Method
		{
			get { return new HoldMethod(); }
		}

	}
}
