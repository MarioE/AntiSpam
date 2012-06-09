using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiSpam
{
    public static class Extensions
    {
        public static bool IsUpper(this string str)
        {
            foreach (char c in str)
            {
                if (!char.IsUpper(c) && !char.IsLetter(c))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
