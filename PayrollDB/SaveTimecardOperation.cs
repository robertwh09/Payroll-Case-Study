using MySql.Data.MySqlClient;
using Payroll;

namespace PayrollMySQLDB
{
   public class SaveTimecardOperation
   {
      private readonly Timecard timeCard;
      private readonly Employee employee;
      private readonly MySqlConnection conn;
      private MySqlCommand insertTimecardCommand;
      public SaveTimecardOperation(Timecard timeCard, Employee employee, MySqlConnection conn)
      {
         this.timeCard = timeCard;
         this.employee = employee;
         this.conn = conn;
      }
      public void Execute()
      {
         //PrepareToSaveTimecard(timecard);
         insertTimecardCommand = CreateInsertTimecardCommand(timeCard, employee);
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

      private MySqlCommand CreateInsertTimecardCommand(Timecard timecard, Employee employee)
      {
         string sql = "insert into Timecard values (@[date], @Hours, @EmpId)";
         MySqlCommand command = new MySqlCommand(sql);
         command.Parameters.AddWithValue("@[date]", timeCard.Date);
         command.Parameters.AddWithValue("@Hours", timeCard.Hours);
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
