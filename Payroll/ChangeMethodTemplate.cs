using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
	public abstract class ChangeMethodTemplate : ChangeEmployeeTemplate
	{
		public ChangeMethodTemplate(int empId, PayrollDatabase database) : base(empId, database)
		{ }

		protected override void Change(Employee e)
		{
			PaymentMethod method = Method;
			e.Method = method;
			database.SaveEmployee(e);
		}

		protected abstract PaymentMethod Method { get; }
	}
}
