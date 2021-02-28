namespace Payroll
{
   public class ChangeUnaffiliatedUseCase : ChangeAffiliationUseCase
   {
      public ChangeUnaffiliatedUseCase(int empId, IPayrollDatabase database) : base(empId, database) { }
      protected override Affiliation Affiliation => new NoAffiliation();

      protected override void RecordMembership(Employee e)
      {
         Affiliation affiliation = e.Affiliation;
         if (affiliation is UnionAffiliation)
         {
            UnionAffiliation unionAffiliation =
            affiliation as UnionAffiliation;
            int memberId = unionAffiliation.MemberId;
            database.RemoveUnionMember(memberId);
         }
      }
   }
}
