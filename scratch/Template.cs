using System.Text;
// namespace will be replaced
namespace T_NAMESPACE
{
	// type name will be replaced
	class T_TYPE
	{
		// init value will be replaced
		public static int[] Primes = null;
		public static string ToString()
		{
			var sb = new StringBuilder();
			for(var i =0;i<Primes.Length;++i)
			{
				if (0 != i)
					sb.Append(", ");
				sb.Append(Primes[i]);
			}
			return sb.ToString();
		}
	}
}
