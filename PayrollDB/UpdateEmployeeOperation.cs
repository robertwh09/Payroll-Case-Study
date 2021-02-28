using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using Payroll;

namespace PayrollMySQLDB
{
   public class UpdateEmployeeOperation
   {
      private readonly Employee employee;
      private readonly MySqlConnection conn;
      private MySqlCommand updateEmployeeCommand;
      public UpdateEmployeeOperation(Employee employee, MySqlConnection conn)
      {
         this.employee = employee;
         this.conn = conn;
      }

      public void Execute()
      {
         PrepareToUpdateEmployee(employee);
         MySqlTransaction transaction = conn.BeginTransaction();
         try
         {
            ExecuteCommand(updateEmployeeCommand, transaction);
            transaction.Commit();
         }
         catch (MySqlException e)
         {
            transaction.Rollback();
            throw e;
         }
      }

      private void PrepareToUpdateEmployee(Employee employee)
      {
         string sql = "update Employee set EmpID = @EmpId, Name = @Name, Address = @Address where EmpID = @EmpId";
         updateEmployeeCommand = new MySqlCommand(sql);
         updateEmployeeCommand.Parameters.AddWithValue("@EmpId", employee.EmpId);
         updateEmployeeCommand.Parameters.AddWithValue("@Name", employee.Name);
         updateEmployeeCommand.Parameters.AddWithValue("@Address", employee.Address);
         //updateEmployeeCommand.Parameters.AddWithValue("@ScheduleType", PaymentScheduleCode(employee.Schedule));
         //updateEmployeeCommand.Parameters.AddWithValue("@PaymentMethodType", paymentMethodCode);
         //updateEmployeeCommand.Parameters.AddWithValue("@PaymentClassificationType", salaryClassificationCode);
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
