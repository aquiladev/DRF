using System.Diagnostics;

namespace Drf.Console
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var drf = new Drf();
			var sw = Stopwatch.StartNew();

			var files = drf.Search(args[0]);
			sw.Stop();

			foreach (var file in files)
			{
				System.Console.WriteLine(file);
			}
			System.Console.WriteLine("ElapsedMilliseconds: {0}", sw.ElapsedMilliseconds);
		}
	}
}
