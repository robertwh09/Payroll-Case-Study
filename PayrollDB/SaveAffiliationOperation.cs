﻿using MySql.Data.MySqlClient;
using Payroll;


namespace PayrollMySQLDB
{
   class SaveAffiliationOperation
   {
      private readonly int affiliationId;
      private readonly Employee employee;
      private readonly MySqlConnection conn;
      private MySqlCommand insertAffiliationCommand;

      public SaveAffiliationOperation(int affiliationId, Employee employee, MySqlConnection conn)
      {
         this.affiliationId = affiliationId;
         this.employee = employee;
         this.conn = conn;
      }

      public void Execute()
      {
         insertAffiliationCommand = CreateInsertAffiliationCommand(affiliationId, employee);
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

      private MySqlCommand CreateInsertAffiliationCommand(int affiliationId, Employee employee)
      {
         string sql = "insert into EmployeeAffiliation values (@EmpId, @Affiliation)";
         MySqlCommand command = new MySqlCommand(sql);
         command.Parameters.AddWithValue("@Affiliation", affiliationId);
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
