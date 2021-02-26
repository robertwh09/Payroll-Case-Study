using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using Payroll;

namespace PayrollDB
{
   class SaveEmployeeOperation
   {
      private readonly Employee employee;
      private readonly MySqlConnection conn;
      private string methodCode;
      private string classificationCode;
      private MySqlCommand insertPaymentMethodCommand;
      private MySqlCommand insertEmployeeCommand;
      private MySqlCommand insertClassificationCommand;
      public SaveEmployeeOperation(Employee employee, MySqlConnection conn)
      {
         this.employee = employee;
         this.conn = conn;
      }
      public void Execute()
      {
         PrepareToSavePaymentMethod(employee);
         PrepareToSaveClassification(employee);
         PrepareToSaveEmployee(employee);

         MySqlTransaction transaction = conn.BeginTransaction();
         try
         {
            ExecuteCommand(insertEmployeeCommand, transaction);
            ExecuteCommand(insertPaymentMethodCommand, transaction);
            ExecuteCommand(insertClassificationCommand, transaction);
            transaction.Commit();
         }
         catch (MySqlException e)
         {
            transaction.Rollback();
            throw e;
         }
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

      private void PrepareToSaveClassification(Employee employee)
      {
         PaymentClassification classification = employee.Classification;
         if (classification is HourlyClassification)
         {
            classificationCode = "hourly";
            HourlyClassification hourlyClassification = classification as HourlyClassification;
            insertClassificationCommand = CreateInsertHourlyClassificationCommand(hourlyClassification, employee);
         }
         else if (classification is SalariedClassification)
         {
            classificationCode = "salary";
            SalariedClassification salariedClassification = classification as SalariedClassification;
            insertClassificationCommand = CreateInsertSalariedClassificationCommand(salariedClassification, employee);
         }
         else if (classification is CommissionedClassification)
         {
            classificationCode = "commission";
            CommissionedClassification commissionClassification = classification as CommissionedClassification;
            insertClassificationCommand = CreateInsertCommissionClassificationCommand(commissionClassification, employee);
         }
         else
            classificationCode = "unknown";
      }

      private MySqlCommand CreateInsertSalariedClassificationCommand(SalariedClassification classification, Employee employee)
      {
         string sql = "insert into SalariedClassification values (@Salary, @EmpId)";
         MySqlCommand command = new MySqlCommand(sql);
         command.Parameters.AddWithValue("@Salary", classification.Salary);
         command.Parameters.AddWithValue("@EmpId", employee.EmpId);
         return command;
      }

      private void PrepareToSaveEmployee(Employee employee)
      {
         string sql = "insert into Employee values (@EmpId, @Name, @Address, @ScheduleType, @PaymentMethodType, @PaymentClassificationType)";
         insertEmployeeCommand = new MySqlCommand(sql);
         insertEmployeeCommand.Parameters.AddWithValue("@EmpId", employee.EmpId);
         insertEmployeeCommand.Parameters.AddWithValue("@Name", employee.Name);
         insertEmployeeCommand.Parameters.AddWithValue("@Address", employee.Address);
         insertEmployeeCommand.Parameters.AddWithValue("@ScheduleType", ScheduleCode(employee.Schedule));
         insertEmployeeCommand.Parameters.AddWithValue("@PaymentMethodType", methodCode);
         insertEmployeeCommand.Parameters.AddWithValue("@PaymentClassificationType", classificationCode);
      }

      private static string ScheduleCode(PaymentSchedule schedule)
      {
         if (schedule is MonthlySchedule)
            return "monthly";
         if (schedule is WeeklySchedule)
            return "weekly";
         if (schedule is BiWeeklySchedule)
            return "biweekly";
         else
            return "unknown";
      }

      private void PrepareToSavePaymentMethod(Employee employee)
      {
         PaymentMethod method = employee.Method;
         if (method is HoldMethod)
            methodCode = "hold";
         else if (method is DirectDepositMethod)
         {
            methodCode = "direct";
            DirectDepositMethod ddMethod = method as DirectDepositMethod;
            insertPaymentMethodCommand = CreateInsertDirectDepositCommand(ddMethod, employee);
         }
         else if (method is MailMethod)
         {
            methodCode = "mail";
            MailMethod mailMethod = method as MailMethod;
            insertPaymentMethodCommand = CreateInsertMailMethodCommand(mailMethod, employee);
         }
         else
            methodCode = "unknown";
      }

      private MySqlCommand CreateInsertCommissionClassificationCommand(CommissionedClassification classification, Employee employee)
      {
         string sql = "insert into CommissionedClassification values (@Salary, @Commission, @EmpId)";
         MySqlCommand command = new MySqlCommand(sql);
         command.Parameters.AddWithValue("@Salary", classification.BaseSalary);
         command.Parameters.AddWithValue("@Commission", classification.CommissionRate);
         command.Parameters.AddWithValue("@EmpId", employee.EmpId);
         return command;
      }

      private MySqlCommand CreateInsertMailMethodCommand(MailMethod mailMethod, Employee employee)
      {
         string sql = "insert into PaycheckAddress " +
         "values (@Address, @EmpId)";
         MySqlCommand command = new MySqlCommand(sql);
         command.Parameters.AddWithValue("@Address", mailMethod.Address);
         command.Parameters.AddWithValue("@EmpId", employee.EmpId);
         return command;
      }

      private MySqlCommand CreateInsertDirectDepositCommand(DirectDepositMethod ddMethod, Employee employee)
      {
         string sql = "insert into directdepositaccount values (@Bank, @Account, @EmpId)";
         MySqlCommand command = new MySqlCommand(sql, conn);
         command.Parameters.AddWithValue("@Bank", ddMethod.Bank);
         command.Parameters.AddWithValue("@Account", ddMethod.AccountNumber);
         command.Parameters.AddWithValue("@EmpId", employee.EmpId);
         return command;
      }

      private MySqlCommand CreateInsertHourlyClassificationCommand(HourlyClassification classification, Employee employee)
      {
         string sql = "insert into HourlyClassification values (@HourlyRate, @EmpId)";
         MySqlCommand command = new MySqlCommand(sql);
         command.Parameters.AddWithValue("@HourlyRate", classification.HourlyRate);
         command.Parameters.AddWithValue("@EmpId", employee.EmpId);
         return command;
      }
   }
}
