namespace Payroll
{
   public class ChangeToUnaffiliatedUseCase : ChangeAffiliationTemplate
   {
      public ChangeToUnaffiliatedUseCase(int empId, PayrollDatabase database) : base(empId, database) { }
      protected override Affiliation Affiliation => new NoAffiliation();

      protected override void RecordMembership(Employee e)
      {
         Affiliation affiliation = e.Affiliation;
         if (affiliation is UnionAffiliation)
         {
            UnionAffiliation unionAffiliation = affiliation as UnionAffiliation;
            int memberId = unionAffiliation.MemberId;
            database.DeleteAffiliateMember(memberId);
         }
      }
   }
}
