namespace Payroll
{
   public class ChangeAffiliationUseCase : ChangeAffiliationTemplate
   {
      private readonly int memberId;
      private readonly double dues;
      public ChangeAffiliationUseCase(int empId, int memberId, double dues, IPayrollDatabase database) : base(empId, database)
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
         database.AddAffiliateMember(memberId, e);
      }
   }
}