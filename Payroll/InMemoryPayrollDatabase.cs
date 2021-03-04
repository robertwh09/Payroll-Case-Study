using System;
using System.Collections;
using System.Collections.Generic;

namespace Payroll
{
   public class InMemoryPayrollDatabase : PayrollDatabase
   {     
      private static Hashtable employees = new Hashtable();
      private static Hashtable employeeAffiliations = new Hashtable();
      //private static Hashtable affiliation = new Hashtable();
      //private static Hashtable serviceCharges = new Hashtable();

      public void SaveEmployee(Employee employee)
      {
         employees[employee.EmpId] = employee;
      }

      public Employee GetEmployee(int id)
      {
         return employees[id] as Employee;
      }

      public void DeleteEmployee(int id)
      {
         employees.Remove(id);
      }


      public ArrayList GetAllEmployeeIds()
      {
         ArrayList arrayList = new ArrayList();
         foreach (DictionaryEntry e in employees)
         {
            arrayList.Add(e.Key);
         }
         return arrayList;
      }

      public IList GetAllEmployees()
      {
         throw new System.NotImplementedException();
      }

      public void UpdateEmployee(Employee employee)
      {
         employees[employee.EmpId] = employee;
      }
      public void Clear()
      {
         //serviceCharges.Clear();
         //affiliation.Clear();
         employeeAffiliations.Clear();
         employees.Clear();
      }

      public void AddAffiliateMember(int memberId, Employee e)
      {
         employeeAffiliations[memberId] = e;
      }

      public Employee GetAffiliateMember(int memberId)
      {
         return employeeAffiliations[memberId] as Employee;
      }

      public void RemoveAffiliateMember(int memberId)
      {
         employeeAffiliations.Remove(memberId);
      }
      void PayrollDatabase.AddAffiliateServiceCharge(int affId, DateTime date, double serviceCharge)
      {

         throw new NotImplementedException();
      }

      ArrayList PayrollDatabase.GetAffiliateServiceCharge(int affId, DateTime startDate, DateTime endDate)
      {
         throw new NotImplementedException();
      }

   }
}
