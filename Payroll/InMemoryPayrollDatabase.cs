using System.Collections;

namespace Payroll
{
   public class InMemoryPayrollDatabase : IPayrollDatabase
   {     
      private static Hashtable employees = new Hashtable();
      private static Hashtable unionMembers = new Hashtable();
      public void AddEmployee(Employee employee)
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
   }
}
