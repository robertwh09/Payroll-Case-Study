using MySql.Data.MySqlClient;
using Payroll;


namespace PayrollMySQLDB
{
   class SaveAffiliationsOperation
   {
      private readonly int affiliationId;
      private readonly int empId;
      private readonly double dues;
      private readonly MySqlConnection conn;
      private MySqlCommand insertAffiliationCommand;

      public SaveAffiliationsOperation(int affiliationId, Employee employee, MySqlConnection conn)
      {
         this.affiliationId = affiliationId;
         this.empId = employee.EmpId;
         this.conn = conn;
      }

      public void Execute()
      {
         insertAffiliationCommand = CreateInsertAffiliationCommand();
         MySqlTransaction transaction = conn.BeginTransaction();
         try
         {
            ExecuteCommand(insertAffiliationCommand, transaction);
            transaction.Commit();
         }
         catch (MySqlException e)
         {
            transaction.Rollback();
            throw e;
         }
      }

      //TODO1 need to add Affiliation and EmployeeAffilliation at the same time.  Review structure
      private MySqlCommand CreateInsertAffiliationCommand()
      {
         string sql = "insert into EmployeeAffiliation(EmpId, AffiliationId) values (@EmpId, @AffiliationId)";
         MySqlCommand command = new MySqlCommand(sql);
         command.Parameters.AddWithValue("@AffiliationId", affiliationId);
         command.Parameters.AddWithValue("@EmpId", empId);
         return command;
      }
      private void ExecuteCommand(MySqlCommand command, MySqlTransaction transaction)
      {
         if (command != null)
         {
            command.Connection = conn;
            command.Transaction = transaction;
            command.ExecuteNonQuery();
         }
      }
   }
}
