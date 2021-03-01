namespace Payroll
{
	public class ChangeCommissionedUseCase: ChangeClassificationUseCase
	{
		private readonly double baseSalary;
		private readonly double commissionRate;

		public ChangeCommissionedUseCase(int id, double baseSalary, double commissionRate, PayrollDatabase database) : base(id, database)
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