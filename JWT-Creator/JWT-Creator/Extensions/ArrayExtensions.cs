using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomExtensions
{
    public static class ArrayExtensions
    {
        public static bool TryGet(this string[] array, int index, out string result)
        {
            try
            {
                result = array[index];
                return true;

            }
            catch (IndexOutOfRangeException e)
            {
                result = string.Empty;
            }

            return false;
        }

        
    }

}
