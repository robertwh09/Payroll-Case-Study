namespace Payroll
{
   public class ChangeHourlyUseCase : ChangeClassificationUseCase
   {
      private readonly double hourlyRate;
      public ChangeHourlyUseCase(int id, double hourlyRate, InMemoryPayrollDatabase database) : base(id, database)
      {
         this.hourlyRate = hourlyRate;
      }
      protected override PaymentClassification Classification
      {
         get { return new HourlyClassification(hourlyRate); }
      }
      protected override PaymentSchedule Schedule
      {
         get { return new WeeklySchedule(); }
      }
   }
}
