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
		ArrayList GetAllEmployeeIds();
		IList GetAllEmployees();

		void AddAffiliateMember(int id, Employee e);
		Employee GetAffiliateMember(int id);
		void RemoveAffiliateMember(int memberId);


		void AddTimecard(int empId, Timecard timecard);
		Timecard GetTimecard(int empId, DateTime date);
		IList GetTimecard(int empId, DateTime startDate, DateTime endDate);
		void RemoveTimecard(int empId, DateTime date);

		/*
		void AddAffiliateDues(int affId, double dues);
		double GetAffiliateDues(int affId);
		void ChangeAffiliateDues(int affId, double dues);
		void RemoveAffiliateDues(int affId, double dues);
		*/

		/*
		void AddAffiliateServiceCharge(int affId, DateTime date, double serviceCharge);
		IList GetAffiliateServiceCharge(int affId, DateTime startDate, DateTime endDate);
		void RemoveAffiliateServiceCharge(int affId, DateTime date, double serviceCharge);
		*/

		/*
		void AddSalesReceipt(int empId, DateTime date, double salesAmount);
		SalesReceipt GetSalesReceipt(int empId, DateTime date);
		IList GetSalesReceipt(int empId, DateTime startDate, DateTime endDate);
		void RemoveSalesReceipt(int empId, DateTime date, double salesAmount);
		*/
	}
}
