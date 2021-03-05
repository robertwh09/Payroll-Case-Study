using MySql.Data.MySqlClient;
using Payroll;

namespace PayrollMySQLDB
{
   public class SaveTimeCardOperation
   {
      private readonly TimeCard timeCard;
      private readonly int empId;
      private readonly MySqlConnection conn;
      private MySqlCommand saveTimeCardCommand;
      public SaveTimeCardOperation(int empId, TimeCard timeCard, MySqlConnection conn)
      {
         this.timeCard = timeCard;
         this.empId = empId;
         this.conn = conn;
      }
      public void Execute()
      {
         PrepareToSaveTimeCard();

         MySqlTransaction transaction = conn.BeginTransaction();
         try
         {
            ExecuteNonQueryCommand(saveTimeCardCommand, transaction);

            transaction.Commit();
         }
         catch (MySqlException e)
         {
            transaction.Rollback();
            throw e;
         }
      }

      private void PrepareToSaveTimeCard()
      {
         LoadTimeCardOperation ltc = new LoadTimeCardOperation(empId, timeCard.Date, conn);

         if (ltc.Execute() == null)
         {
            saveTimeCardCommand = CreateInsertTimecardCommand(timeCard);
         }

         else
         {
            saveTimeCardCommand = CreateUpdateTimecardCommand(timeCard);
         }
      }

      private void ExecuteNonQueryCommand(MySqlCommand command, MySqlTransaction transaction)
      {
         if (command != null)
         {
            command.Connection = conn;
            command.Transaction = transaction;
            command.ExecuteNonQuery();
         }
      }
      private MySqlCommand CreateInsertTimecardCommand(TimeCard timecard)
      {
         string sql = "insert into Timecard(Date, EmpID, Hours) values(@Date, @EmpId, @Hours)";
         MySqlCommand command = new MySqlCommand(sql);
         command.Parameters.AddWithValue("@Date", timecard.Date);
         command.Parameters.AddWithValue("@Hours", timecard.Hours);
         command.Parameters.AddWithValue("@EmpId", empId);
         return command;
      }

      private MySqlCommand CreateUpdateTimecardCommand(TimeCard timecard)
      {
         string sql = "UPDATE Timecard SET Date=@Date, EmpID=@EmpId, Hours=@Hours WHERE EmpID=@EmpId AND Date=@Date";
         MySqlCommand command = new MySqlCommand(sql);
         command.Parameters.AddWithValue("@Date", timecard.Date);
         command.Parameters.AddWithValue("@Hours", timecard.Hours);
         command.Parameters.AddWithValue("@EmpId", empId);
         return command;
      }
   }
}

