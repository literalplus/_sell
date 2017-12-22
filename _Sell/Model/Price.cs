namespace _Sell.Model
{
    public class Price
    {
        private static readonly int MINORS_PER_MAJOR = 100;
        private int major;
        private int minor;

        public Price(int major, int minor)
        {
            this.major = major;
            this.minor = minor;
        }

        public Price(int rawValue)
        {
            this.minor = rawValue % MINORS_PER_MAJOR;
            this.major = (rawValue - this.minor) / MINORS_PER_MAJOR;
        }

        public int Major
        {
            get { return major; }
        }

        public int Minor
        {
            get { return minor; }
        }

        public int RawValue
        {
            get { return major * MINORS_PER_MAJOR + minor; }
        }

        public Price Plus(Price other)
        {
            return new Price(RawValue + other.RawValue);
        }

        public Price Minus(Price other)
        {
            return new Price(RawValue - other.RawValue);
        }

        public Price Times(int factor)
        {
            return new Price(RawValue * factor);
        }

        public override string ToString()
        {
            return string.Format("€ {0:D2},{1:D2}", Major, Minor);
        }
    }
}