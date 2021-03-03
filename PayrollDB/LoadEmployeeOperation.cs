using MySql.Data.MySqlClient;
using Payroll;
using System;
using System.Data;

namespace PayrollMySQLDB
{
   public class LoadEmployeeOperation
   {
      private readonly int empId;
      private readonly MySqlConnection connection;

      public LoadEmployeeOperation(int empId, MySqlConnection connection)
      {
         this.empId = empId;
         this.connection = connection;
      }
      public MySqlCommand LoadEmployeeCommand
      {
         get
         {
            string sql = "select * from Employee where EmpId=@EmpId";
            MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EmpId", empId);
            return command;
         }
      }

      internal void Execute()
      {
         string sql = "select *  from Employee where EmpId = @EmpId";
         MySqlCommand command = new MySqlCommand(sql, connection);
         command.Parameters.AddWithValue("@EmpId", empId);

         DataRow row = LoadDataFromCommand(command);

         CreateEmployee(row);
         AddSchedule(row);
         AddPaymentMethod(row);
         AddClassification(row);
      }

      public void AddSchedule(DataRow row)
      {
         string scheduleType = row["ScheduleType"].ToString();
         if (scheduleType.Equals("weekly"))
            Employee.Schedule = new WeeklySchedule();
         else if (scheduleType.Equals("biweekly"))
            Employee.Schedule = new BiWeeklySchedule();
         else if (scheduleType.Equals("monthly"))
            Employee.Schedule = new MonthlySchedule();
      }

      public void AddPaymentMethod(DataRow row)
      {
         string methodCode = row["PaymentMethodType"].ToString();
         LoadPaymentMethodOperation operation = new LoadPaymentMethodOperation(Employee, methodCode, connection);
         operation.Execute();
         Employee.Method = operation.Method;
      }

      public void AddClassification(DataRow row)
      {
         string classificationCode = row["PaymentClassificationType"].ToString();
         LoadPaymentClassificationOperation operation = new LoadPaymentClassificationOperation(Employee, classificationCode, connection);
         operation.Execute();
         Employee.Classification = operation.Classification;
      }
      public void CreateEmployee(DataRow row)
      {
         string name = row["Name"].ToString();
         string address = row["Address"].ToString();
         Employee = new Employee(empId, name, address);
      }
      public static DataRow LoadDataFromCommand(MySqlCommand command)
      {
         MySqlDataAdapter adapter = new MySqlDataAdapter(command);//TODO error deleting table
         DataSet dataset = new DataSet();
         adapter.Fill(dataset);
         DataTable table = dataset.Tables["table"];
         return table.Rows[0];
      }
      public Employee Employee { get; set; }
   }
}