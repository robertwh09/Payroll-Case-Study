using MySql.Data.MySqlClient;

namespace PayrollMySQLDB
{
   public class RemoveAffiliateMemberOperation
   {
      private readonly int memberId;
      private readonly MySqlConnection connection;
      public RemoveAffiliateMemberOperation(int memberId, MySqlConnection connection)
      {
         this.memberId = memberId;
         this.connection = connection;
      }

      public MySqlCommand RemoveAffiliationCommand
      {
         get
         {
            string sql = "delete from EmployeeAffiliation where AffiliationId = @AffpId";
            MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@AffpId", memberId);
            return command;
         }
      }
      public void Execute()
      {  
         //remove affiliation
         MySqlTransaction transaction = connection.BeginTransaction();
         try
         {
            ExecuteCommand(RemoveAffiliationCommand, transaction);
            transaction.Commit();
         }
         catch (MySqlException e)
         {
            transaction.Rollback();
            throw e;
         }

         //remove servicecharge
         //remove employeeaffiliation
      }

      private void ExecuteCommand(MySqlCommand command, MySqlTransaction transaction)
      {
         if (command != null)
         {
            command.Connection = connection;
            command.Transaction = transaction;
            command.ExecuteNonQuery();
         }
      }
   }
}
