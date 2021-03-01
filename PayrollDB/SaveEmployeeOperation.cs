using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;
using Payroll;

namespace PayrollMySQLDB
{
   class SaveEmployeeOperation
   {
      public static readonly int MYSQL_DUPLICATE_PK = 1062;
      private readonly Employee employee;
      private readonly MySqlConnection conn;
      private string paymentMethodCode;
      private string salaryClassificationCode;
      private MySqlCommand insertPaymentMethodCommand;
      private MySqlCommand saveEmployeeCommand;
      private MySqlCommand insertSalaryClassificationCommand;
      private MySqlCommand queryEmployeeExistsCommand;

      public SaveEmployeeOperation(Employee employee, MySqlConnection conn)
      {
         this.employee = employee;
         this.conn = conn;
      }
      public void Execute()
      {
         PrepareToSavePaymentMethod(employee);
         PrepareToSaveSalaryClassification(employee);
         //check if employee already exists in database
         
         if (QueryEmployeeExists(employee))
         {
            PrepareToUpdateEmployee(employee);
         }
         else
         {
            PrepareToInsertEmployee(employee);
         }

         MySqlTransaction transaction = conn.BeginTransaction();
         try
         {
            ExecuteNonQueryCommand(saveEmployeeCommand, transaction);
            ExecuteNonQueryCommand(insertPaymentMethodCommand, transaction);
            ExecuteNonQueryCommand(insertSalaryClassificationCommand, transaction);
            transaction.Commit();
         }
         catch (MySqlException e)
         {
            transaction.Rollback();
            throw e;
         }
      }

      private bool QueryEmployeeExists(Employee employee)
      {
         string sql = "SELECT TOP 1 Employee.EmpId FROM Employee WHERE employee.EmpId=@EmpId";
         queryEmployeeExistsCommand = new MySqlCommand(sql);
         queryEmployeeExistsCommand.Parameters.AddWithValue("@EmpId", employee.EmpId);

         LoadEmployeeOperation lo = new LoadEmployeeOperation(employee.EmpId, conn);
         try
         {
            lo.Execute();
         }
         catch
         {
            return false;
         }
            return true;
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

      private void PrepareToSaveSalaryClassification(Employee employee)
      {
         PaymentClassification classification = employee.Classification;
         if (classification is HourlyClassification)
         {
            salaryClassificationCode = "hourly";
            HourlyClassification hourlyClassification = classification as HourlyClassification;
            insertSalaryClassificationCommand = CreateInsertHourlyClassificationCommand(hourlyClassification, employee);
            HourlyClassification hc = classification as HourlyClassification;
         }
         else if (classification is SalariedClassification)
         {
            salaryClassificationCode = "salary";
            SalariedClassification salariedClassification = classification as SalariedClassification;
            insertSalaryClassificationCommand = CreateInsertSalariedClassificationCommand(salariedClassification, employee);
         }
         else if (classification is CommissionedClassification)
         {
            salaryClassificationCode = "commission";
            CommissionedClassification commissionClassification = classification as CommissionedClassification;
            insertSalaryClassificationCommand = CreateInsertCommissionClassificationCommand(commissionClassification, employee);
         }
         else
            salaryClassificationCode = "unknown";
      }

      private MySqlCommand CreateInsertSalariedClassificationCommand(SalariedClassification classification, Employee employee)
      {
         string sql = "insert into SalariedClassification values (@Salary, @EmpId)";
         MySqlCommand command = new MySqlCommand(sql);
         command.Parameters.AddWithValue("@Salary", classification.Salary);
         command.Parameters.AddWithValue("@EmpId", employee.EmpId);
         return command;
      }

      private void PrepareToInsertEmployee(Employee employee)
      {
         string sql = "insert into Employee values (@EmpId, @Name, @Address, @ScheduleType, @PaymentMethodType, @PaymentClassificationType)";
         saveEmployeeCommand = new MySqlCommand(sql);
         saveEmployeeCommand.Parameters.AddWithValue("@EmpId", employee.EmpId);
         saveEmployeeCommand.Parameters.AddWithValue("@Name", employee.Name);
         saveEmployeeCommand.Parameters.AddWithValue("@Address", employee.Address);
         saveEmployeeCommand.Parameters.AddWithValue("@ScheduleType", PaymentScheduleCode(employee.Schedule));
         saveEmployeeCommand.Parameters.AddWithValue("@PaymentMethodType", paymentMethodCode);
         saveEmployeeCommand.Parameters.AddWithValue("@PaymentClassificationType", salaryClassificationCode);
      }

      private void PrepareToUpdateEmployee(Employee employee)
      {
         string sql = "update Employee set EmpID = @EmpId, Name=@Name, Address=@Address, ScheduleType=@ScheduleType," +
            "PaymentMethodType=@PaymentMethodType, PaymentClassificationType=@PaymentClassificationType where EmpID = @EmpId";
         saveEmployeeCommand = new MySqlCommand(sql);
         saveEmployeeCommand.Parameters.AddWithValue("@EmpId", employee.EmpId);
         saveEmployeeCommand.Parameters.AddWithValue("@Name", employee.Name);
         saveEmployeeCommand.Parameters.AddWithValue("@Address", employee.Address);
         saveEmployeeCommand.Parameters.AddWithValue("@ScheduleType", PaymentScheduleCode(employee.Schedule));
         saveEmployeeCommand.Parameters.AddWithValue("@PaymentMethodType", paymentMethodCode);
         saveEmployeeCommand.Parameters.AddWithValue("@PaymentClassificationType", salaryClassificationCode);
      }

      private static string PaymentScheduleCode(PaymentSchedule schedule)
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
            paymentMethodCode = "hold";
         else if (method is DirectDepositMethod)
         {
            paymentMethodCode = "direct";
            DirectDepositMethod ddMethod = method as DirectDepositMethod;
            insertPaymentMethodCommand = CreateInsertDirectDepositCommand(ddMethod, employee);
         }
         else if (method is MailMethod)
         {
            paymentMethodCode = "mail";
            MailMethod mailMethod = method as MailMethod;
            insertPaymentMethodCommand = CreateInsertMailMethodCommand(mailMethod, employee);
         }
         else
            paymentMethodCode = "unknown";
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
         string sql = "insert into PaycheckAddress values (@Address, @EmpId)";
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
