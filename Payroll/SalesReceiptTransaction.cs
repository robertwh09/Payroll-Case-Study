using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
   public class SalesReceiptTransaction : Transaction
	{
		private readonly DateTime date;
		private readonly double saleAmount;
		private readonly int empId;

		public SalesReceiptTransaction(DateTime time, double saleAmount, int empId)
		{
			this.date = time;
			this.saleAmount = saleAmount;
			this.empId = empId;
		}

		public void Execute()
		{
			Employee e = PayrollDatabase.GetEmployee(empId);

			if (e != null)
			{
				CommissionedClassification cc = e.Classification as CommissionedClassification;

				if (cc != null)
					cc.AddSalesReceipt(new SalesReceipt(date, saleAmount));
				else
					throw new ApplicationException(
						"Tried to add sales receipt to" +
							"non-commissioned employee");
			}
			else
				throw new ApplicationException(
					"No such employee.");

		}
	}
}
