using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
	public class ChangeMailUseCase : ChangeMethodTemplate
	{
		private readonly string address;
		public ChangeMailUseCase(int empId, string address, PayrollDatabase database) : base(empId, database)
		{
			this.address = address;
		}

		protected override PaymentMethod Method
		{
			get { return new MailMethod(address); }
		}

	}
}
