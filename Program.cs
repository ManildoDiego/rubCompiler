using rub.RubParser;

namespace rub
{
    public class Program 
	{
		static public readonly DiagnosticBag errors = new("compiler");

		public static void Main()
		{
			string filePath = "main.rub";
			
			try 
			{
				Run(filePath);
            }
			catch (IOException i)
			{
				errors.FileNotExists(i.Message[21..^2]);
				errors.Print();
			}
			catch (SystemException err)
	        {
				errors.CompilerError(err.Message);
				errors.Print();
	        }

			Console.Read();
		}

		public static void Run(string filePath)
		{
			var lines = Functions.GetLinesFrom(filePath);
			errors.Print();

			Parser parser = new(lines);
			parser.Parse();
		}
	}
}