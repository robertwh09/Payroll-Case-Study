using MySql.Data.MySqlClient;
using Payroll;
using System;

namespace PayrollMySQLDB
{
   class SaveAffiliationsOperation
   {
      private readonly int affiliationId;
      private readonly Employee employee;
      private readonly MySqlConnection conn;
      private MySqlCommand insertAffiliationCommand;

      public SaveAffiliationsOperation(int affiliationId, Employee employee, MySqlConnection conn)
      {
         this.affiliationId = affiliationId;
         this.employee = employee;
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

      private void GetAffiliations()
      {
         //TODO1 need to add Affiliation and EmployeeAffilliation at the same time.  Review structure
         UnionAffiliation un = employee.Affiliation as UnionAffiliation;
         int memberId = un.MemberId;
         double dues = un.Dues;
         ServiceCharge serviceCharge = un.GetServiceCharge(new DateTime(2021, 1, 1));
      }
      
      private MySqlCommand CreateInsertAffiliationCommand()
      {
         string sql = "insert into EmployeeAffiliation(EmpId, AffiliationId) values (@EmpId, @AffiliationId)";
         MySqlCommand command = new MySqlCommand(sql);
         command.Parameters.AddWithValue("@AffiliationId", affiliationId);
         command.Parameters.AddWithValue("@EmpId", employee.EmpId);
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
