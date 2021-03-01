using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
	public abstract class ChangeMethodUseCase : ChangeEmployeeTemplate
	{
		public ChangeMethodUseCase(int empId, PayrollDatabase database) : base(empId, database)
		{ }

		protected override void Change(Employee e)
		{
			PaymentMethod method = Method;
			e.Method = method;
		}

		protected abstract PaymentMethod Method { get; }
	}
}
