namespace PayrollMySQLDB
{
   interface DatabaseCommand
   {
      void Validate();
      void Execute();
      void Undo();
   }
}
