using System.Collections;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Payroll.Database
{
   public class SqlPayrollDatabase : IPayrollDatabase
   {
      private readonly MySql.Data.MySqlClient.MySqlConnection conn;
      public SqlPayrollDatabase()
      {
         string connString = "server=localhost;user id=sa;database=payroll;pwd=abc";
         conn = new MySql.Data.MySqlClient.MySqlConnection(connString);
         conn.Open();
      }
      public void AddEmployee(Employee employee)
      {
         string sql = "insert into employee values (" +
         "@EmpId, @Name, @Address, @ScheduleType, " +
         "@PaymentMethodType, @PaymentClassificationType)";

         MySqlCommand command = new MySqlCommand();
         command.Connection = conn;
         command.CommandText = sql;

         command.Parameters.AddWithValue("@EmpId", employee.EmpId);
         command.Parameters.AddWithValue("@Name", employee.Name);
         command.Parameters.AddWithValue("@Address", employee.Address);
         command.Parameters.AddWithValue("@ScheduleType", ScheduleCode(employee.Schedule));
         command.Parameters.AddWithValue("@PaymentMethodType", employee.Method.GetType().ToString());
         command.Parameters.AddWithValue("@PaymentClassificationType", employee.Classification.GetType().ToString());
         command.ExecuteNonQuery();
      }
      private static string ScheduleCode(PaymentSchedule schedule)
      {
         if (schedule is MonthlySchedule)
            return "monthly";
         else
            return "unknown";
      }

      public void AddUnionMember(int id, Employee e)
      {
         throw new System.NotImplementedException();
      }

      public void DeleteEmployee(int id)
      {
         throw new System.NotImplementedException();
      }

      public ArrayList GetAllEmployeeIds()
      {
         throw new System.NotImplementedException();
      }

      public IList GetAllEmployees()
      {
         throw new System.NotImplementedException();
      }

      public Employee GetEmployee(int id)
      {
         throw new System.NotImplementedException();
      }

      public Employee GetUnionMember(int id)
      {
         throw new System.NotImplementedException();
      }

      public void RemoveUnionMember(int memberId)
      {
         throw new System.NotImplementedException();
      }
   }
}