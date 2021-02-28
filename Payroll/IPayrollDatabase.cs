using System;
using System.Collections;

namespace Payroll
{
   public interface IPayrollDatabase
   {
		void CreateEmployee(Employee employee);
		void UpdateEmployee(Employee employee);
		Employee GetEmployee(int id);
		void DeleteEmployee(int id);
		void AddUnionMember(int id, Employee e);
		Employee GetUnionMember(int id);
		void RemoveUnionMember(int memberId);
		ArrayList GetAllEmployeeIds();
		IList GetAllEmployees();

		void AddTimecard(int empId, Timecard timecard);
		Timecard GetTimecard(int empId, DateTime date);
		IList GetTimecardByDateRange(int empId, DateTime startDate, DateTime endDate);
		void RemoveTimecard(int empId, DateTime date);

   }
}
