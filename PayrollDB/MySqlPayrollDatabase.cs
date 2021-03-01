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
         SaveAffiliationOperation operation = new SaveAffiliationOperation(memberId, employee, conn);
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

      public Employee GetEmployee(int id)
      {
         LoadEmployeeOperation loadOperation = new LoadEmployeeOperation(id, conn);
         loadOperation.Execute();
         return loadOperation.Employee;
      }

      public Employee GetAffiliateMember(int affId)
      {
         try
         {
            //TODO why are we using the Command pattern here?
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

      public void AddTimecard(int empId, Timecard timecard)
      {
         //TODO need to add support to Add Timecards
         throw new System.NotImplementedException();
      }
      public Timecard GetTimecard(int empId, DateTime date)
      {
         //TODO need to add support to Get Timecards by Date
         throw new System.NotImplementedException();
      }
      public IList GetTimecard(int empId, DateTime startDate, DateTime endDate)
      {
         //TODO need to add support to Get Timecards by Date range
         throw new System.NotImplementedException();
      }
      public void RemoveTimecard(int empId, DateTime date)
      {
         //TODO need to add support to Delete Timecards
         throw new System.NotImplementedException();
      }

      public void UpdateEmployee(Employee employee)
      {
         UpdateEmployeeOperation operation = new UpdateEmployeeOperation(employee, conn);
         operation.Execute();
      }
   }
}