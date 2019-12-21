using System;
using System.Text;

namespace xytools
{
    static class xy_str
    {
        public static string Reverse(string input)
        {
            string output = "";
            int i = 1;
            while (i <= input.Length)
            {
                output += input[input.Length - i];
                i++;
            }
            return output;
        }
        /// <summary>
        /// explodes string[] from string. Optionally, a custom seperator can be chosen.
        /// </summary>
        /// <param name="str">The string to explode</param>
        /// <param name="seperator">Custom Seperator(optional), default ','</param>
        /// <returns>string[] with exploded strings form str</returns>
        /// <exception cref="System.ArgumentException" />
        /// <exception cref="System.ArgumentNullException" />
        public static string[] explode(string str, char seperator = ',')
        {
            if (str == "") throw new ArgumentException("xytools.xy_str.explode(str,sep): str must not be empty");
            //if (seperator == null) throw new ArgumentNullException("xytools.xy_str.explode(str,sep): sep must not be null!");
            if (str == null) throw new ArgumentNullException("xytools.xy_str.explode(str,sep): str must not be null");
            string[] output = new string[xy_str.countCharOccurences(str, seperator)];
            string TempString = "";
            byte i = 0;
            foreach (char letter in str)
            {
                if (letter == seperator)
                {
                    output[i] = TempString;
                    i++;
                    TempString = "";
                }
                else
                {
                    TempString += letter;
                }
            }
            return output;
        }
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
        /// Converts String-Array to String with sep between items.
        /// </summary>
        /// <param name="arr">Array to convert</param>
        /// <param name="sep">Custom seperator (optional)</param>
        /// <returns>Output String</returns>
        public static string implode(string[] arr, char sep = ',')
        {
            string output = "";
            foreach (string item in arr)
            {
                output += item + sep;
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
