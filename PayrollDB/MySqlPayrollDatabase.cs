using System;
using System.Collections;
using Payroll;

namespace PayrollMySQLDB
{
   public class MySqlPayrollDatabase : PayrollDatabase
   {
      private readonly MySql.Data.MySqlClient.MySqlConnection conn;
      
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

      public void SaveEmployee(Employee employee)
      {
         SaveEmployeeOperation operation = new SaveEmployeeOperation(employee, conn);
         operation.Execute();
      }

      public void AddAffiliateMember(int memberId, Employee employee)
      {
         SaveAffiliationsOperation operation = new SaveAffiliationsOperation(memberId, employee, conn);
         operation.Execute();
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

      public Employee GetEmployee(int empId)
      {
         LoadEmployeeOperation loadOperation = new LoadEmployeeOperation(empId, conn);
         loadOperation.Execute();
         return loadOperation.Employee;
      }

      public Employee GetAffiliateMember(int affId)
      {
         try
         {
            //TODO3 why are we using the Command pattern here?
            GetEmpIDFromAffiliateIDOperation loadOperation = new GetEmpIDFromAffiliateIDOperation(affId, conn);
            loadOperation.Execute();
            return GetEmployee(loadOperation.EmpId);
         }
         catch (Exception)
         {
            return null;
         }
      }

      public void RemoveAffiliateMember(int memberId)
      {
         RemoveAffiliateMemberOperation removeOperation = new RemoveAffiliateMemberOperation(memberId, conn);
         removeOperation.Execute();
      }
   }
}