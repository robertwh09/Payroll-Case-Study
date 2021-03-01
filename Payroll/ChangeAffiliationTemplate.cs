namespace Payroll
{
   //Template Pattern used here.  All changes require the affiliation to be updated
   //But the the abstract RecordMembership() allows the derivatives to manage what needs to be changed
   public abstract class ChangeAffiliationTemplate : ChangeEmployeeTemplate
   {
      public ChangeAffiliationTemplate(int empId, PayrollDatabase database) : base(empId, database) { }
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