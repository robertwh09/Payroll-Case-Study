﻿using System;
using System.Collections;
using MySql.Data.MySqlClient;
using Payroll;

namespace PayrollMySQLDB
{
   public class MySqlPayrollDatabase : IPayrollDatabase
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

      public void CreateEmployee(Employee employee)
      {
         CreateEmployeeOperation operation = new CreateEmployeeOperation(employee, conn);
         operation.Execute();
      }

      public void AddUnionMember(int id, Employee e)
      {
         throw new System.NotImplementedException();
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

      public Employee GetUnionMember(int id)
      {
         throw new System.NotImplementedException();
      }

      public void RemoveUnionMember(int memberId)
      {
         throw new System.NotImplementedException();
      }

      public void AddTimecard(int empId, Timecard timecard)
      {
         throw new System.NotImplementedException();
      }
      public Timecard GetTimecard(int empId, DateTime date)
      {
         throw new System.NotImplementedException();
      }
      public IList GetTimecardByDateRange(int empId, DateTime startDate, DateTime endDate)
      {
         throw new System.NotImplementedException();
      }
      public void RemoveTimecard(int empId, DateTime date)
      {
         throw new System.NotImplementedException();
      }

      public void UpdateEmployee(Employee employee)
      {
         UpdateEmployeeOperation operation = new UpdateEmployeeOperation(employee, conn);
         operation.Execute();
      }
   }
}