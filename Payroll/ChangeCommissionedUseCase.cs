namespace Payroll
{
	public class ChangeCommissionedUseCase: ChangeClassificationTemplate
	{
		private readonly double baseSalary;
		private readonly double commissionRate;

		public ChangeCommissionedUseCase(int empId, double baseSalary, double commissionRate, PayrollDatabase database) : base(empId, database)
		{
			this.baseSalary = baseSalary;
			this.commissionRate = commissionRate;
		}

		protected override PaymentClassification Classification
		{
			get { return new CommissionedClassification(baseSalary, commissionRate); }
		}

		protected override PaymentSchedule Schedule
		{
			get { return new BiWeeklySchedule(); }
		}
	}
}