using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
	public class ChangeSalaryUseCase : ChangeClassificationUseCase
	{
		private readonly double salary;

		public ChangeSalaryUseCase(int id, double salary, IPayrollDatabase database) : base(id, database)
		{
			this.salary = salary;
		}

		protected override PaymentClassification Classification
		{
			get { return new SalariedClassification(salary); }
		}

		protected override PaymentSchedule Schedule
		{
			get { return new MonthlySchedule(); }
		}
	}
}
