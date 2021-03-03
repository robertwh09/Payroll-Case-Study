using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
	public class ChangeDirectDepositUseCase : ChangeMethodTemplate
	{
		private readonly string bank;
		private readonly string accountNumber;
		public ChangeDirectDepositUseCase(int empId, string bank, string account, PayrollDatabase database) : base(empId, database)
		{
			this.bank = bank;
			this.accountNumber = account;
		}

		protected override PaymentMethod Method
		{
			get { return new DirectDepositMethod(bank, accountNumber); }
		}

	}
}
