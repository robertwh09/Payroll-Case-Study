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
      private MySqlCommand saveNewPaymentMethodCommand;
      private MySqlCommand deleteExistingPaymentMethodCommand = null;
      private MySqlCommand saveNewSalaryClassificationCommand;
      private MySqlCommand deleteExistingSalaryClassificationCommand = null;
      private MySqlCommand queryEmployeeExistsCommand;
      private readonly Employee employeeToSave;
      private Employee existingEmployee;
      private bool employeeInDB = false;

      public SaveEmployeeOperation(Employee employee, MySqlConnection conn)
      {
         this.employeeToSave = employee;
         this.conn = conn;
      }
      public void Execute()
      {
         EmployeeExists();
         PrepareToSavePaymentMethod();
         PrepareToSaveSalaryClassification();
             
         if (employeeInDB)
         {
            PrepareToUpdateEmployee();
         }
         else
         {
            PrepareToInsertEmployee();
         }

         MySqlTransaction transaction = conn.BeginTransaction();
         try
         {
            ExecuteNonQueryCommand(saveEmployeeCommand, transaction);
            ExecuteNonQueryCommand(deleteExistingPaymentMethodCommand, transaction);
            ExecuteNonQueryCommand(saveNewPaymentMethodCommand, transaction);
            ExecuteNonQueryCommand(deleteExistingSalaryClassificationCommand, transaction);
            ExecuteNonQueryCommand(saveNewSalaryClassificationCommand, transaction);
            transaction.Commit();
         }
         catch (MySqlException e)
         {
            transaction.Rollback();
            throw e;
         }
      }

      private void EmployeeExists()
      {
         string sql = "SELECT TOP 1 EmpId FROM Employee WHERE EmpId=@EmpId";
         queryEmployeeExistsCommand = new MySqlCommand(sql);
         queryEmployeeExistsCommand.Parameters.AddWithValue("@EmpId", employeeToSave.EmpId);

         LoadEmployeeOperation loadEmployeeOperation = new LoadEmployeeOperation(employeeToSave.EmpId, conn);
         try
         {
            loadEmployeeOperation.Execute();
         }
         catch
         {
            existingEmployee = null;
            employeeInDB = false;
            return;
         }
         employeeInDB = true;
         existingEmployee = loadEmployeeOperation.Employee;
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

      //#####################################################################
      //                EMPLOYEE METHODS
      //#####################################################################
      private void PrepareToInsertEmployee()
      {
         string sql = "insert into Employee values (@EmpId, @Name, @Address, @ScheduleType, @PaymentMethodType, @PaymentClassificationType)";

         LoadEmployeeSQLParameters(sql);
      }
      private void PrepareToUpdateEmployee()
      {
         string sql = "update Employee set EmpID = @EmpId, Name=@Name, Address=@Address, ScheduleType=@ScheduleType," +
            "PaymentMethodType=@PaymentMethodType, PaymentClassificationType=@PaymentClassificationType where EmpID=@EmpId";

         LoadEmployeeSQLParameters(sql);
      }

      private void LoadEmployeeSQLParameters(string sql)
      {
         saveEmployeeCommand = new MySqlCommand(sql);
         saveEmployeeCommand.Parameters.AddWithValue("@EmpId", employeeToSave.EmpId);
         saveEmployeeCommand.Parameters.AddWithValue("@Name", employeeToSave.Name);
         saveEmployeeCommand.Parameters.AddWithValue("@Address", employeeToSave.Address);
         saveEmployeeCommand.Parameters.AddWithValue("@ScheduleType", PaymentScheduleCode(employeeToSave.Schedule));
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

      //#####################################################################
      //                SALARY CLASSIFICATION METHODS
      //#####################################################################
      private void PrepareToSaveSalaryClassification()
      {
         if (!employeeInDB)//employee not in db so need to insert all data
         {
            CreateClassificationInsertCommand();
         }
         else if (employeeInDB && existingEmployee.Classification.GetType() != employeeToSave.Classification.GetType())//employee in db and classification changing
         {
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
            CreateClassificationInsertCommand();//create insert for new classification
         }
         else 
         {
            CreateClassificationUpdateCommand();//classification not changing but values might be, need to create update
         }
      }

      private MySqlCommand LoadCommissionedSQLParameters(CommissionedClassification classification, string sql)
      {
         MySqlCommand command = new MySqlCommand(sql);
         command.Parameters.AddWithValue("@Salary", classification.Salary);
         command.Parameters.AddWithValue("@Commission", classification.CommissionRate);
         command.Parameters.AddWithValue("@EmpId", employeeToSave.EmpId);
         return command;
      }
      private MySqlCommand LoadSalariedSQLParameters(SalariedClassification classification, string sql)
      {
         MySqlCommand command = new MySqlCommand(sql);
         command.Parameters.AddWithValue("@Salary", classification.Salary);
         command.Parameters.AddWithValue("@EmpId", employeeToSave.EmpId);
         return command;
      }
      private MySqlCommand LoadHourlySQLParameters(HourlyClassification classification, string sql)
      {
         MySqlCommand command = new MySqlCommand(sql);
         command.Parameters.AddWithValue("@HourlyRate", classification.HourlyRate);
         command.Parameters.AddWithValue("@EmpId", employeeToSave.EmpId);
         return command;
      }

      //INSERT METHODS
      private void CreateClassificationInsertCommand()
      {
         PaymentClassification classificationToSave = employeeToSave.Classification;
         if (classificationToSave is HourlyClassification)
         {
            salaryClassificationCode = "hourly";
            HourlyClassification hourlyClassification = classificationToSave as HourlyClassification;
            saveNewSalaryClassificationCommand = CreateInsertHourlyClassificationCommand(hourlyClassification);
         }
         else if (classificationToSave is SalariedClassification)
         {
            salaryClassificationCode = "salary";
            SalariedClassification salariedClassification = classificationToSave as SalariedClassification;
            saveNewSalaryClassificationCommand = CreateInsertSalariedClassificationCommand(salariedClassification);
         }
         else if (classificationToSave is CommissionedClassification)
         {
            salaryClassificationCode = "commission";
            CommissionedClassification commissionClassification = classificationToSave as CommissionedClassification;
            saveNewSalaryClassificationCommand = CreateInsertCommissionClassificationCommand(commissionClassification);
         }
         else
            salaryClassificationCode = "unknown";
      }
      private MySqlCommand CreateInsertSalariedClassificationCommand(SalariedClassification classification)
      {
         string sql = "insert into SalariedClassification values (@Salary, @EmpId)";
         MySqlCommand command = LoadSalariedSQLParameters(classification, sql);
         return command;
      }

      private MySqlCommand CreateInsertCommissionClassificationCommand(CommissionedClassification classification)
      {
         string sql = "insert into CommissionedClassification values (@Salary, @Commission, @EmpId)";
         MySqlCommand command = LoadCommissionedSQLParameters(classification, sql);
         return command;
      }

      private MySqlCommand CreateInsertHourlyClassificationCommand(HourlyClassification classification)
      {
         string sql = "insert into HourlyClassification values (@HourlyRate, @EmpId)";
         MySqlCommand command = LoadHourlySQLParameters(classification, sql);
         return command;
      }

      //UPDATE METHODS
      private void CreateClassificationUpdateCommand()
      {
         PaymentClassification classificationToSave = employeeToSave.Classification;
         if (classificationToSave is HourlyClassification)
         {
            salaryClassificationCode = "hourly";
            HourlyClassification hourlyClassification = classificationToSave as HourlyClassification;
            saveNewSalaryClassificationCommand = CreateUpdateHourlyClassificationCommand(hourlyClassification);
         }
         else if (classificationToSave is SalariedClassification)
         {
            salaryClassificationCode = "salary";
            SalariedClassification salariedClassification = classificationToSave as SalariedClassification;
            saveNewSalaryClassificationCommand = CreateUpdateSalariedClassificationCommand(salariedClassification);
         }
         else if (classificationToSave is CommissionedClassification)
         {
            salaryClassificationCode = "commission";
            CommissionedClassification commissionClassification = classificationToSave as CommissionedClassification;
            saveNewSalaryClassificationCommand = CreateUpdateCommissionClassificationCommand(commissionClassification);
         }
         else
            salaryClassificationCode = "unknown";
      }
      private MySqlCommand CreateUpdateCommissionClassificationCommand(CommissionedClassification classification)
      {
         string sql = "update ComissionedClassification set Salary=@Salary, Commission=@Commission, EmpId=@EmpId where EmpID = @EmpId";
         MySqlCommand command = LoadCommissionedSQLParameters(classification, sql);
         return command;
      }
      private MySqlCommand CreateUpdateSalariedClassificationCommand(SalariedClassification classification)
      {
         string sql = "update SalariedClassification set Salary=@Salary, EmpId=@EmpId where EmpID = @EmpId";
         MySqlCommand command = LoadSalariedSQLParameters(classification, sql);
         return command;
      }

      private MySqlCommand CreateUpdateHourlyClassificationCommand(HourlyClassification classification)
      {
         string sql = "update SalariedClassification set HourlyRate=@HourlyRate, EmpId=@EmpId where EmpID = @EmpId";
         MySqlCommand command = LoadHourlySQLParameters(classification, sql);
         return command;
      }

      //DELETE METHODS
      private void CreateDeleteComissionedClassificationCommand()
      {
         string sql = "delete from CommissionedClassification where EmpID=@EmpId";
         deleteExistingSalaryClassificationCommand = new MySqlCommand(sql);
         deleteExistingSalaryClassificationCommand.Parameters.AddWithValue("@EmpId", existingEmployee.EmpId);
      }

      private void CreateDeleteSalaryClassificationCommand()
      {
         string sql = "delete from SalariedClassification where EmpID=@EmpId";
         deleteExistingSalaryClassificationCommand = new MySqlCommand(sql);
         deleteExistingSalaryClassificationCommand.Parameters.AddWithValue("@EmpId", existingEmployee.EmpId);
      }

      private void CreateDeleteHourlyClassificationCommand()
      {
         string sql = "delete from HourlyClassification where EmpID=@EmpId";
         deleteExistingSalaryClassificationCommand = new MySqlCommand(sql);
         deleteExistingSalaryClassificationCommand.Parameters.AddWithValue("@EmpId", existingEmployee.EmpId);
      }

      //#####################################################################
      //                PAYMENT METHOD METHODS
      //#####################################################################
      private void PrepareToSavePaymentMethod()
      {
       //if existing need to delete old method before adding new one

         if (!employeeInDB)//employee not in db so need to insert all data
         {
            CreateInsertMethodCommand();
         }
         else if (employeeInDB && existingEmployee.Method.GetType() != employeeToSave.Method.GetType())//employee in db and classification changing
         {
            PaymentMethod methodToDelete = existingEmployee.Method;
            if (methodToDelete is DirectDepositMethod)
            {
               DirectDepositMethod ddMethod = methodToDelete as DirectDepositMethod;
               deleteExistingPaymentMethodCommand = CreateDeleteDirectDepositCommand();
            }
            else if (methodToDelete is MailMethod)
            {
               MailMethod mailMethod = methodToDelete as MailMethod;
               deleteExistingPaymentMethodCommand = CreateDeleteMailMethodCommand();
            }
            CreateInsertMethodCommand();
         }
         else
         {
            CreateUpdateMethodCommand();
         }
      }

      //INSERT METHODS
      private void CreateInsertMethodCommand()
      {
         PaymentMethod methodToSave = employeeToSave.Method;
         if (methodToSave is HoldMethod)
            paymentMethodCode = "hold";
         else if (methodToSave is DirectDepositMethod)
         {
            paymentMethodCode = "direct";
            DirectDepositMethod ddMethod = methodToSave as DirectDepositMethod;
            saveNewPaymentMethodCommand = CreateInsertDirectDepositCommand(ddMethod);
         }
         else if (methodToSave is MailMethod)
         {
            paymentMethodCode = "mail";
            MailMethod mailMethod = methodToSave as MailMethod;
            saveNewPaymentMethodCommand = CreateInsertMailMethodCommand(mailMethod);
         }
         else
            paymentMethodCode = "unknown";
      }
      private MySqlCommand CreateInsertMailMethodCommand(MailMethod mailMethod)
      {
         string sql = "insert into PaycheckAddress values (@Address, @EmpId)";
         MySqlCommand command = new MySqlCommand(sql);
         command.Parameters.AddWithValue("@Address", mailMethod.Address);
         command.Parameters.AddWithValue("@EmpId", employeeToSave.EmpId);
         return command;
      }

      private MySqlCommand CreateInsertDirectDepositCommand(DirectDepositMethod ddMethod)
      {
         string sql = "insert into directdepositaccount values (@Bank, @Account, @EmpId)";
         MySqlCommand command = new MySqlCommand(sql, conn);
         command.Parameters.AddWithValue("@Bank", ddMethod.Bank);
         command.Parameters.AddWithValue("@Account", ddMethod.AccountNumber);
         command.Parameters.AddWithValue("@EmpId", employeeToSave.EmpId);
         return command;
      }

      //UPDATE METHODS
      private void CreateUpdateMethodCommand()
      {
         PaymentMethod methodToSave = employeeToSave.Method;
         if (methodToSave is HoldMethod)
            paymentMethodCode = "hold";
         else if (methodToSave is DirectDepositMethod)
         {
            paymentMethodCode = "direct";
            DirectDepositMethod ddMethod = methodToSave as DirectDepositMethod;
            saveNewPaymentMethodCommand = CreateUpdateDirectDepositCommand(ddMethod);
         }
         else if (methodToSave is MailMethod)
         {
            paymentMethodCode = "mail";
            MailMethod mailMethod = methodToSave as MailMethod;
            saveNewPaymentMethodCommand = CreateUpdateMailMethodCommand(mailMethod);
         }
         else
            paymentMethodCode = "unknown";
      }

      private MySqlCommand CreateUpdateMailMethodCommand(MailMethod mailMethod)
      {
         string sql = "update PaycheckAddress set Address=@Address where EmpID=@EmpId";
         MySqlCommand command = new MySqlCommand(sql);
         command.Parameters.AddWithValue("@Address", mailMethod.Address);
         command.Parameters.AddWithValue("@EmpId", employeeToSave.EmpId);
         return command;
      }

      private MySqlCommand CreateUpdateDirectDepositCommand(DirectDepositMethod ddMethod)
      {
         string sql = "update DirectDepositAccount set Bank=@Bank, Account=@Account where EmpID=@EmpId";
         MySqlCommand command = new MySqlCommand(sql);
         command.Parameters.AddWithValue("@Bank", ddMethod.Bank);
         command.Parameters.AddWithValue("@Account", ddMethod.AccountNumber);
         command.Parameters.AddWithValue("@EmpId", employeeToSave.EmpId);
         return command;
      }

      //DELETE METHODS
      private MySqlCommand CreateDeleteMailMethodCommand()
      {
         string sql = "delete from PaycheckAddress where Empid=@EmpId";
         MySqlCommand command = new MySqlCommand(sql);
         command.Parameters.AddWithValue("@EmpId", existingEmployee.EmpId);
         return command;
      }

      private MySqlCommand CreateDeleteDirectDepositCommand()
      {
         string sql = "delete from DirectDepositAccount where Empid=@EmpId";
         MySqlCommand command = new MySqlCommand(sql);
         command.Parameters.AddWithValue("@EmpId", existingEmployee.EmpId);
         return command;
      }
   }
}
