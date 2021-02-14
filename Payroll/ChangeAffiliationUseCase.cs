namespace Payroll
{
   public abstract class ChangeAffiliationUseCase : ChangeEmployeeUseCase
   {
      public ChangeAffiliationUseCase(int empId, InMemoryPayrollDatabase database) : base(empId, database) { }
      protected override void Change(Employee e)
      {
         RecordMembership(e);
         Affiliation affiliation = Affiliation;
         e.Affiliation = affiliation;
      }
      protected abstract Affiliation Affiliation { get; }
      protected abstract void RecordMembership(Employee e);
   }
}