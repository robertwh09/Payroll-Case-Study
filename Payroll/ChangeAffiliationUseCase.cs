namespace Payroll
{
   public abstract class ChangeAffiliationUseCase : ChangeEmployeeUseCase
   {
      public ChangeAffiliationUseCase(int empId) : base(empId) { }
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