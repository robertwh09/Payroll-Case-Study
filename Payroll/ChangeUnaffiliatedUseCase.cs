namespace Payroll
{
   public class ChangeUnaffiliatedUseCase : ChangeAffiliationUseCase
   {
      public ChangeUnaffiliatedUseCase(int empId) : base(empId) { }
      protected override Affiliation Affiliation => new NoAffiliation();

      protected override void RecordMembership(Employee e)
      {
         Affiliation affiliation = e.Affiliation;
         if (affiliation is UnionAffiliation)
         {
            UnionAffiliation unionAffiliation =
            affiliation as UnionAffiliation;
            int memberId = unionAffiliation.MemberId;
            PayrollDatabase.RemoveUnionMember(memberId);
         }
      }
   }
}
