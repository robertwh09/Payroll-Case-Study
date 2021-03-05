using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class SalesReceiptUseCase : UseCase
	{
		private readonly DateTime date;
		private readonly double saleAmount;
		private readonly int empId;

		public SalesReceiptUseCase(DateTime time, double saleAmount, int empId, PayrollDatabase database) : base (database)
		{
			this.date = time;
			this.saleAmount = saleAmount;
			this.empId = empId;
		}

		public override void Execute()
		{
			Employee e = database.GetEmployee(empId);

			if (e != null)
			{
				CommissionedClassification cc = e.Classification as CommissionedClassification;

				if (cc != null)
					cc.AddSalesReceipt(new SalesReceipt(date, saleAmount));
				else
					throw new ApplicationException("Tried to add sales receipt to non-commissioned employee");
			}
			else
				throw new ApplicationException("No such employee.");

		}
	}
}
