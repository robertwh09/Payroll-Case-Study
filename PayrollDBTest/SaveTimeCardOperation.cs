using MySql.Data.MySqlClient;
using Payroll;

namespace PayrollMySQLDB.Tests
{
   class SaveTimeCardOperation
   {
      private readonly TimeCard timecard;
      private readonly int empId;
      private readonly MySqlConnection conn;
      private MySqlCommand insertTimecardCommand;

      public SaveTimeCardOperation(TimeCard timecard, Employee employee, MySqlConnection conn)
      {
         this.timecard = timecard;
         this.empId = employee.EmpId;
         this.conn = conn;
      }

      public void Execute()
      {
         insertTimecardCommand = CreateInsertTimeCardCommand(timecard, empId);
         MySqlTransaction transaction = conn.BeginTransaction();
         try
         {
            ExecuteCommand(insertTimecardCommand, transaction);
            transaction.Commit();
         }
         catch (MySqlException e)
         {
            transaction.Rollback();
            throw e;
         }
      }

      private MySqlCommand CreateInsertTimeCardCommand(TimeCard timecard, int empId)
      {
         string sql = "insert into TimeCard values (@[date], @Hours, @EmpId)";
         MySqlCommand command = new MySqlCommand(sql);
         command.Parameters.AddWithValue("@[date]", timecard.Date);
         command.Parameters.AddWithValue("@Hours", timecard.Hours);
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
