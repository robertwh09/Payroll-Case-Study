using System;
using System.Collections;
using System.Collections.Generic;

namespace Payroll
{
   public class InMemoryPayrollDatabase : PayrollDatabase
   {     
      private static Hashtable employees = new Hashtable();
      private static Hashtable unionMembers = new Hashtable();
      private static IDictionary timecardDict = new Dictionary<Tuple<int, DateTime>, Timecard>();
      private static Hashtable affiliationDues = new Hashtable();
      private static Hashtable serviceCharges = new Hashtable();

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

      public void AddAffiliateMember(int memberId, Employee employee)
      {
         unionMembers[memberId] = employee;
      }

      public Employee GetAffiliateMember(int memberId)
      {
         return unionMembers[memberId] as Employee;
      }

      public void RemoveAffiliateMember(int memberId)
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


      public IList GetTimecard(int empId, DateTime startDate, DateTime endDate)
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

      public void UpdateEmployee(Employee employee)
      {
         employees[employee.EmpId] = employee;
      }
      public void Clear()
      {
         employees.Clear();
         unionMembers.Clear();
         timecardDict.Clear();
      }
   }
}
