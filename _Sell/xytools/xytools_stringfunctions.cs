using System;
using System.Text;

namespace xytools
{
    static class XyStr
    {
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
        public static string ListArrayToString(Array arr, ushort howManyTabs = 1)
        {
            var tabz = XyStr.Multiply("   ", howManyTabs);
            D.W("###xy_str.listArrayToString(): Lenght=" + arr.Length);
            try
            {
                var output = arr.GetType().ToString() + "{\n";
                if (arr is string[])
                {
                    var i = 0;
                    foreach (string item in arr)
                    {
                        output += tabz + i + " => '" + item + "', \n";
                        i++;
                    }
                }
                else
                {
                    var j = 0;
                    foreach (var item in arr)
                    {
                        if (item is Array array)
                        {
                            output += tabz + j + " => Array: \n" + XyStr.ListArrayToString(array) + ", \n";
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
            var i = 0;
            var sb = new StringBuilder();
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
            var result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}
