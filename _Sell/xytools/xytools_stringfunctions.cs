using System;
using System.Text;

namespace xytools
{
    static class xy_str
    {
        public static int countCharOccurences(string str, char sep = ',')
        {
            int output = 0;
            foreach (char item in str)
            {
                if (item == sep) output++;
            }
            return output;
        }

        /// <summary>
        /// returns an array as a string. i.e.
        /// string[]{
        ///      0 => '12',
        ///      1 => '13',
        ///  }
        /// </summary>
        /// <param name="howManyTabs">how many tabs to add in front of items</param>
        /// <param name="arr">source array</param>
        /// <returns>arr as string</returns>
        public static string listArrayToString(Array arr, ushort howManyTabs = 1)
        {
            string tabz = xy_str.Multiply("   ", howManyTabs);
            D.W("###xy_str.listArrayToString(): Lenght=" + arr.Length);
            try
            {
                string output = arr.GetType().ToString() + "{\n";
                if (arr is string[])
                {
                    int i = 0;
                    foreach (string item in arr)
                    {
                        output += tabz + i + " => '" + item + "', \n";
                        i++;
                    }
                }
                else
                {
                    int j = 0;
                    foreach (object item in arr)
                    {
                        if (item is Array)
                        {
                            output += tabz + j + " => Array: \n" + xy_str.listArrayToString((Array)item) + ", \n";
                        }
                        else
                        {
                            try
                            {
                                output += tabz + j + " => '" + item.ToString() + "', \n";
                            }
                            catch (NullReferenceException)
                            {
                                output += tabz + j + " => null/static, \n";
                            }
                        }
                        j++;
                    }
                }
                return output + "}";
            }
            catch (Exception ex)
            {
                D.W("xy_str.listArrayToString(): Exception " + ex.Message);
                //throw ex;
                return "Error";
            }
        }

        /// <summary>
        /// returns str chained together times times.
        /// </summary>
        /// <param name="str">string</param>
        /// <param name="times">how often?</param>
        /// <returns>output</returns>
        public static string Multiply(string str, int times)
        {
            int i = 0;
            StringBuilder sb = new StringBuilder();
            while (i < times)
            {
                sb.Append(str);
                i++;
            }
            return sb.ToString();
        }

        //http://stackoverflow.com/a/943650/1117552
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}
