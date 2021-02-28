using MySql.Data.MySqlClient;

namespace PayrollMySQLDB
{
   public class GetEmpIDFromAffiliateIDOperation
   {
      private readonly int affId;
      private readonly MySqlConnection connection;
      public int EmpId { get; set; }
      public GetEmpIDFromAffiliateIDOperation(int affId, MySqlConnection connection)
      {
         this.affId = affId;
         this.connection = connection;
      }

      public void Execute()
      {
         MySqlCommand command = new MySqlCommand("select EmpId from EmployeeAffiliation where AffiliationId = @affId", connection);
         command.Parameters.AddWithValue("@affId", affId);

         MySqlDataReader dr = command.ExecuteReader();
         dr.Read();
         EmpId = (int)dr.GetValue(0);
         dr.Close();
      }
   }
}
