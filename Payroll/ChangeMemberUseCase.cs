namespace Payroll
{
   public class ChangeMemberUseCase : ChangeAffiliationUseCase
   {
      private readonly int memberId;
      private readonly double dues;
      public ChangeMemberUseCase(int empId, int memberId, double dues, InMemoryPayrollDatabase database) : base(empId, database)
      {
         this.memberId = memberId;
         this.dues = dues;
      }
      protected override Affiliation Affiliation
      {
         get { return new UnionAffiliation(memberId, dues); }
      }
      protected override void RecordMembership(Employee e)
      {
         database.AddUnionMember(memberId, e);
      }
   }
}