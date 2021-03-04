using MySql.Data.MySqlClient;
using Payroll;
using System;
using System.Collections;

namespace PayrollMySQLDB
{
   public class SaveTimeCardsOperation
   {
      private readonly Employee employee;
      private readonly MySqlConnection conn;
      private MySqlCommand insertTimecardCommand;
      private MySqlCommand deleteTimecardCommand;
      public SaveTimeCardsOperation(Employee employee, MySqlConnection conn)
      {
         if (employee.Classification is HourlyClassification)
         {
            this.employee = employee;
            this.conn = conn;
         }
         else
         {
            throw (new Exception("Employee must be of type HourlyClassification"));
         }
      }
      public void Execute()
      {

         HourlyClassification hc = employee.Classification as HourlyClassification;
         Hashtable timecards = hc.GetAllTimeCards();

         deleteTimecardCommand = CreateDeleteTimecardCommand();

         MySqlTransaction transaction = conn.BeginTransaction();
         try
         {
            //TODO3 review a lazy load approach for timecards where they are loaded and persisted as each change occurs in them
            ExecuteNonQueryCommand(deleteTimecardCommand, transaction);//delete the existing timecards for this employee

            //insert all of the timecards for this employee
            foreach(DictionaryEntry entry in timecards)
            {
               insertTimecardCommand = CreateInsertTimecardCommand((TimeCard) entry.Value);
               ExecuteNonQueryCommand(insertTimecardCommand, transaction);
            }
            transaction.Commit();
         }
         catch (MySqlException e)
         {
            transaction.Rollback();
            throw e;
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
         command.Parameters.AddWithValue("@EmpId", employee.EmpId);
         return command;
      }

      private MySqlCommand CreateDeleteTimecardCommand()
      {
         string sql = "delete from Timecard where EmpId=@EmpId";
         MySqlCommand command = new MySqlCommand(sql);
         command.Parameters.AddWithValue("@EmpId", employee.EmpId);
         return command;
      }
   }
}
