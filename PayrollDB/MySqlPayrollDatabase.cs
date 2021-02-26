using System;
using System.Collections;
using MySql.Data;
using MySql.Data.MySqlClient;
using Payroll;

namespace PayrollDB
{
   public class MySqlPayrollDatabase : IPayrollDatabase
   {
      private string methodCode;
      private string classificationCode;
      private readonly MySql.Data.MySqlClient.MySqlConnection conn;
      private MySqlCommand insertPaymentMethodCommand;
      private MySqlCommand insertClassificationCommand;
      private MySqlCommand insertEmployeeCommand;
      
      public MySqlPayrollDatabase()
      {
         string connString = "server=localhost;user id=sa;database=payroll;pwd=abc";
         conn = new MySql.Data.MySqlClient.MySqlConnection(connString);
         conn.Open();
      }

      ~MySqlPayrollDatabase()
      {
         conn.Close();
      }

      public void AddEmployee(Employee employee)
      {
         SaveEmployeeOperation operation = new SaveEmployeeOperation(employee, conn);
         operation.Execute();
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
         LoadEmployeeOperation loadOperation = new LoadEmployeeOperation(id, conn);
         loadOperation.Execute();
         return loadOperation.Employee;
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