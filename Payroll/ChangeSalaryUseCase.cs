using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll
{
	public class ChangeSalaryUseCase : ChangeClassificationTemplate
	{
		private readonly double salary;

		public ChangeSalaryUseCase(int id, double salary, PayrollDatabase database) : base(id, database)
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
