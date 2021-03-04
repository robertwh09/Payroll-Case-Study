using System;
using System.Collections;

namespace Payroll
{
   public interface PayrollDatabase
   {
		void SaveEmployee(Employee employee);
		Employee GetEmployee(int id);
		void DeleteEmployee(int id);
		ArrayList GetAllEmployeeIds();
		IList GetAllEmployees();

		void AddAffiliateMember(int affilationId, Employee e);
		Employee GetAffiliateMember(int affilationId);
		void RemoveAffiliateMember(int affilationId);

		//TODO1 need to add Service Charges
		/*
		void AddAffiliateServiceCharge(int affId, DateTime date, double serviceCharge);
		IList GetAffiliateServiceCharge(int affId, DateTime startDate, DateTime endDate);
		*/

		//TODO1 need to add Sales Receipts
		/*
		void AddSalesReceipt(int empId, DateTime date, double salesAmount);
		SalesReceipt GetSalesReceipt(int empId, DateTime date);
		IList GetSalesReceipt(int empId, DateTime startDate, DateTime endDate);
		*/
	}
}
