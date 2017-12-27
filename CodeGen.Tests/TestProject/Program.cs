using System;
using System.Collections.Generic;

namespace TestProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
    
	public class PublicClass 
	{
	    public void A()
	    {
	    }
		
	    private void B()
	    {
	    }

		public int Int()
		{
			
		}
		
		public void Parameters(int parameter1, string parameter2, double parameter3, decimal parameter4)
		{
			
		}

		public void Lists(List<int> p0, IEnumerable<int> p1, IReadOnlyList<int> rl, int[] array)
		{
			
		}
		
		public int AProperty { get; }
	}
}
