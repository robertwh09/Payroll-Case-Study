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
		void DeleteAffiliateMember(int affilationId);

		void AddTimeCard(int empId, TimeCard timecard);
		TimeCard GetTimeCard(int empId, DateTime date);
		void DeleteTimeCard(int empId, DateTime date);

		//ArrayList GetTimeCards(Employee employee);
		//ArrayList GetTimeCards(Employee employee, DateTime startDate, DateTime dateTime);

		//TODO1 need to add Service Charges
		void AddAffiliateServiceCharge(int affId, DateTime date, double serviceCharge);
		ArrayList GetAffiliateServiceCharge(int affId, DateTime startDate, DateTime endDate);
		

		//TODO1 need to add Sales Receipts
		/*
		void AddSalesReceipt(int empId, DateTime date, double salesAmount);
		SalesReceipt GetSalesReceipt(int empId, DateTime date);
		IList GetSalesReceipt(int empId, DateTime startDate, DateTime endDate);
		*/
	}
}
