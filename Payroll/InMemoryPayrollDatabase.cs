using System;
using System.Collections;
using System.Collections.Generic;

namespace Payroll
{
   public class InMemoryPayrollDatabase : IPayrollDatabase
   {     
      private static Hashtable employees = new Hashtable();
      private static Hashtable unionMembers = new Hashtable();
      private static Hashtable timecards = new Hashtable();
      private static IDictionary timecardDict = new Dictionary<Tuple<int, DateTime>, Timecard>();
      public void CreateEmployee(Employee employee)
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

      public void AddUnionMember(int id, Employee e)
      {
         unionMembers[id] = e;
      }

      public Employee GetUnionMember(int id)
      {
         return unionMembers[id] as Employee;
      }

      public void RemoveUnionMember(int memberId)
      {
         unionMembers.Remove(memberId);
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

      public void AddTimecard(int empId, Timecard timecard)
      {
         Tuple<int, DateTime> key = new Tuple<int, DateTime>(empId, timecard.Date);
         timecardDict.Add(key, timecard);
      }

      public Timecard GetTimecard(int empId, DateTime date)
      {
         Tuple<int, DateTime> key = new Tuple<int, DateTime>(empId, date);
         Timecard tc = (Timecard)timecardDict[key];
         return tc;
      }


      public IList GetTimecardByDateRange(int empId, DateTime startDate, DateTime endDate)
      {

         ArrayList arrayList = new ArrayList();
         foreach (DictionaryEntry e in timecardDict)
         {
            arrayList.Add(e.Key);
         }
         return arrayList;
      }

      public void RemoveTimecard(int empId, DateTime date)
      {
         Tuple<int, DateTime> key = new Tuple<int, DateTime>(empId, date);
         timecardDict.Remove(key);
      }

      void ChangeEmployee(Employee employee)
      {
         throw new NotImplementedException();
      }

      public void UpdateEmployee(Employee employee)
      {
         employees.Remove(employee.EmpId);
         employees.Add(employee.EmpId, employee);
      }
   }
}
