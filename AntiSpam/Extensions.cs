using System.Collections.Generic;

namespace AntiSpam
{
	public static class Extensions
	{
		public static int GetCount(this string str, char c)
		{
			int charCount = 0;
			foreach (char chr in str)
			{
				if (chr == c)
				{
					charCount++;
				}
			}
			return charCount;
		}
		public static int GetUnique(this string str)
		{
			List<char> charsFound = new List<char>();
			foreach (char c in str)
			{
				if (char.IsLetter(c) && !charsFound.Contains(c))
				{
					charsFound.Add(c);
					charsFound.Add(char.ToUpper(c));
				}
			}
			return charsFound.Count / 2;
		}
		public static double UpperCount(this string str)
		{
			double capsCount = 0;
			double charCount = 0;
			foreach (char c in str)
			{
				if (char.IsLetter(c))
				{
					charCount++;
					if (char.IsUpper(c))
					{
						capsCount++;
					}
				}
			}
			return capsCount / charCount;
		}
	}
}