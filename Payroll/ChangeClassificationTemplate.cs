namespace Payroll
{
   //base class to support all change types to the Salary Classification
   public abstract class ChangeClassificationTemplate : ChangeEmployeeTemplate
   {
      public ChangeClassificationTemplate(int id, PayrollDatabase database) : base(id, database)
      { }
      protected override void Change(Employee e)
      {
         e.Classification = Classification;
         e.Schedule = Schedule;
         database.SaveEmployee(e);
      }
      protected abstract PaymentClassification Classification { get; }
      protected abstract PaymentSchedule Schedule { get; }
   }
}