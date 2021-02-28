using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
	public class ChangeMailUseCase : ChangeMethodUseCase
	{
		public ChangeMailUseCase(int empId, IPayrollDatabase database) : base(empId, database)
		{
		}

		protected override PaymentMethod Method
		{
			get { return new MailMethod("3.14 Pi St"); }
		}

	}
}
