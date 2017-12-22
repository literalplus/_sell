using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace _Sell
{
    public class Price
    {
        public int Cents;
        public double Euros;
        //public int FullEuros;
        public Price()
        {
            this.Cents = 0; this.Euros = 0;
        }
        public Price(int cents)
        {
            this.Cents = cents;
            //MessageBox.Show((cents / 100).ToString()+";"+cents.ToString());
            this.Euros = Convert.ToDouble(cents) / 100;
        }
        public override string ToString()
        {
            return Euros.ToString("C2");
        }
        public static Price Add(Price a, Price b)
        {
            return new Price((a.Cents + b.Cents));
        }
        public static Price Multiply(Price pr, int times)
        {
            return new Price((pr.Cents * times));
        }
        /// <summary>
        /// subtracts subtrahend from minuend.
        /// </summary>
        /// <param name="minuend">minuend</param>
        /// <param name="subtrahend">subtrahend</param>
        /// <returns>difference</returns>
        public static Price Subtract(Price minuend, Price subtrahend)
        {
            return new Price(minuend.Cents - subtrahend.Cents);
        }
    }
}
