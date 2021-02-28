namespace Payroll
{
   //base class to support all change types to the Salary Classification
   public abstract class ChangeClassificationUseCase : ChangeEmployeeTemplate
   {
      public ChangeClassificationUseCase(int id, IPayrollDatabase database) : base(id, database)
      { }
      protected override void Change(Employee e)
      {
         e.Classification = Classification;
         e.Schedule = Schedule;
      }
      protected abstract PaymentClassification Classification { get; }
      protected abstract PaymentSchedule Schedule { get; }
   }
}