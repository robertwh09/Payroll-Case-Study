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
      private readonly MySqlConnection conn;
      private string paymentMethodCode;
      private string salaryClassificationCode;
      private MySqlCommand saveEmployeeCommand;
      private MySqlCommand insertPaymentMethodCommand;
      private MySqlCommand deletePaymentMethodCommand = null;
      private MySqlCommand saveSalaryClassificationCommand;
      private MySqlCommand deleteSalaryClassificationCommand = null;
      private MySqlCommand queryEmployeeExistsCommand;
      private readonly Employee employeeToSave;
      private Employee existingEmployee;
      private bool employeeInDB = false;


      //changes to fields are implemented by specific changeOperation methods
      //this function is called to persist those changes to the employee table only

      //need a strategy for
      //1 inserting new employee
      //2 changing employee table attributes
      //3 changing classification, which will be adding to one table and deleting from another
      //4 changing payment method, which will be adding to one table and deleting from another (there is no table associated with hold however)
      //5 adding or removing an affiliation.  this will need to add/remove from employeeAfffiliation table, add/remove from affiliation table, remove from service charge table
      //as each change comes from a changeOperation which knows what is changing do we use this to direct the change or do we detect the type of change in this method?
      public SaveEmployeeOperation(Employee employee, MySqlConnection conn)
      {
         this.employeeToSave = employee;
         this.conn = conn;
      }
      public void Execute()
      {
         EmployeeExists(employeeToSave);
         PrepareToSavePaymentMethod(employeeToSave);
         PrepareToSaveSalaryClassification(employeeToSave);
             
         if (EmployeeExists(employeeToSave))
         {
            PrepareToUpdateEmployee(employeeToSave);
         }
         else
         {
            PrepareToInsertEmployee(employeeToSave);
         }

         MySqlTransaction transaction = conn.BeginTransaction();
         try
         {
            ExecuteNonQueryCommand(saveEmployeeCommand, transaction);
            ExecuteNonQueryCommand(deletePaymentMethodCommand, transaction);
            ExecuteNonQueryCommand(insertPaymentMethodCommand, transaction);
            ExecuteNonQueryCommand(deleteSalaryClassificationCommand, transaction);
            ExecuteNonQueryCommand(saveSalaryClassificationCommand, transaction);
            transaction.Commit();
         }
         catch (MySqlException e)
         {
            transaction.Rollback();
            throw e;
         }
      }

      private bool EmployeeExists(Employee employee)
      {
         string sql = "SELECT TOP 1 Employee.EmpId FROM Employee WHERE employee.EmpId=@EmpId";
         queryEmployeeExistsCommand = new MySqlCommand(sql);
         queryEmployeeExistsCommand.Parameters.AddWithValue("@EmpId", employee.EmpId);

         LoadEmployeeOperation loadEmployeeOperation = new LoadEmployeeOperation(employee.EmpId, conn);
         try
         {
            loadEmployeeOperation.Execute();
         }
         catch
         {
            return false;
         }
         employeeInDB = true;
         existingEmployee = loadEmployeeOperation.Employee;
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
         //TODO1 check if newclassification = old classification
         //if so do nothing
         //if different create command to insert newclassification and delete old classification

         if (!employeeInDB)
         {
            CreateClassificationInsertCommand(employeeToSave);
         }
         else if (employeeInDB && existingEmployee.Classification.GetType() != employeeToSave.Classification.GetType())
         {  //if here we need to delete the old classification and insert the new one
            PaymentClassification classificationToDelete = existingEmployee.Classification;
            if (classificationToDelete is HourlyClassification)
            {
               CreateDeleteHourlyClassificationCommand();
            }
            else if (classificationToDelete is SalariedClassification)
            {
               CreateDeleteSalaryClassificationCommand();
            }
            else if (classificationToDelete is CommissionedClassification)
            {
               CreateDeleteComissionedClassificationCommand();
            }

            CreateClassificationInsertCommand(employeeToSave);
         }
         else //classification not changing but values might be, need to create update
         {
            //TODO1 create classification update
            CreateClassificationUpdateCommand(employeeToSave);
         }
      }

      private void CreateClassificationUpdateCommand(Employee employee)
      {
         PaymentClassification classificationToSave = employeeToSave.Classification;
         if (classificationToSave is HourlyClassification)
         {
            salaryClassificationCode = "hourly";
            HourlyClassification hourlyClassification = classificationToSave as HourlyClassification;
            saveSalaryClassificationCommand = CreateUpdateHourlyClassificationCommand(hourlyClassification, employee);
            HourlyClassification hc = classificationToSave as HourlyClassification;
         }
         else if (classificationToSave is SalariedClassification)
         {
            salaryClassificationCode = "salary";
            SalariedClassification salariedClassification = classificationToSave as SalariedClassification;
            saveSalaryClassificationCommand = CreateUpdateSalariedClassificationCommand(salariedClassification, employee);
         }
         else if (classificationToSave is CommissionedClassification)
         {
            salaryClassificationCode = "commission";
            CommissionedClassification commissionClassification = classificationToSave as CommissionedClassification;
            saveSalaryClassificationCommand = CreateUpdateCommissionClassificationCommand(commissionClassification, employee);
         }
         else
            salaryClassificationCode = "unknown";
      }

      private MySqlCommand CreateUpdateCommissionClassificationCommand(CommissionedClassification classification, Employee employee)
      {
         string sql = "update ComissionedClassification set Salary=@Salary, Commission=@Commission, EmpId=@EmpId where EmpID = @EmpId";
         MySqlCommand command = new MySqlCommand(sql);
         command.Parameters.AddWithValue("@Salary", classification.Salary);
         command.Parameters.AddWithValue("@Commission", classification.CommissionRate);
         command.Parameters.AddWithValue("@EmpId", employee.EmpId);
         return command;
      }

      private MySqlCommand CreateUpdateSalariedClassificationCommand(SalariedClassification classification, Employee employee)
      {
         string sql = "update SalariedClassification set Salary=@Salary, EmpId=@EmpId where EmpID = @EmpId";
         MySqlCommand command = new MySqlCommand(sql);
         command.Parameters.AddWithValue("@Salary", classification.Salary);
         command.Parameters.AddWithValue("@EmpId", employee.EmpId);
         return command;
      }

      private MySqlCommand CreateUpdateHourlyClassificationCommand(HourlyClassification classification, Employee employee)
      {
         string sql = "update SalariedClassification set HourlyRate=@HourlyRate, EmpId=@EmpId where EmpID = @EmpId";
         MySqlCommand command = new MySqlCommand(sql);
         command.Parameters.AddWithValue("@HourlyRate", classification.HourlyRate);
         command.Parameters.AddWithValue("@EmpId", employee.EmpId);
         return command;
      }

      private void CreateClassificationInsertCommand(Employee employee)
      {
         PaymentClassification classificationToSave = employeeToSave.Classification;
         if (classificationToSave is HourlyClassification)
         {
            salaryClassificationCode = "hourly";
            HourlyClassification hourlyClassification = classificationToSave as HourlyClassification;
            saveSalaryClassificationCommand = CreateInsertHourlyClassificationCommand(hourlyClassification, employee);
            HourlyClassification hc = classificationToSave as HourlyClassification;
         }
         else if (classificationToSave is SalariedClassification)
         {
            salaryClassificationCode = "salary";
            SalariedClassification salariedClassification = classificationToSave as SalariedClassification;
            saveSalaryClassificationCommand = CreateInsertSalariedClassificationCommand(salariedClassification, employee);
         }
         else if (classificationToSave is CommissionedClassification)
         {
            salaryClassificationCode = "commission";
            CommissionedClassification commissionClassification = classificationToSave as CommissionedClassification;
            saveSalaryClassificationCommand = CreateInsertCommissionClassificationCommand(commissionClassification, employee);
         }
         else
            salaryClassificationCode = "unknown";
      }

      private void CreateDeleteComissionedClassificationCommand()
      {
         string sql = "delete from CommissionedClassification where EmpID=@EmpId";
         deleteSalaryClassificationCommand = new MySqlCommand(sql);
         deleteSalaryClassificationCommand.Parameters.AddWithValue("@EmpId", existingEmployee.EmpId);
      }

      private void CreateDeleteSalaryClassificationCommand()
      {
         string sql = "delete from SalariedClassification where EmpID=@EmpId";
         deleteSalaryClassificationCommand = new MySqlCommand(sql);
         deleteSalaryClassificationCommand.Parameters.AddWithValue("@EmpId", existingEmployee.EmpId);
      }

      private void CreateDeleteHourlyClassificationCommand()
      {
         string sql = "delete from HourlyClassification where EmpID=@EmpId";
         deleteSalaryClassificationCommand = new MySqlCommand(sql);
         deleteSalaryClassificationCommand.Parameters.AddWithValue("@EmpId", existingEmployee.EmpId);
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
      {//TODO3 check in new method = existing method
         //if so do nothing
         //if not create command to delete old method and create command to insert new method

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
         command.Parameters.AddWithValue("@Salary", classification.Salary);
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
