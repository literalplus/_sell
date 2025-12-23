namespace _Sell.Model
{
    public class Price
    {
        private const int MinorsPerMajor = 100;
        private readonly int _major;
        private readonly int _minor;

        public Price(int major, int minor)
        {
            this._major = major;
            this._minor = minor;
        }

        public Price(int rawValue)
        {
            this._minor = rawValue % MinorsPerMajor;
            this._major = (rawValue - this._minor) / MinorsPerMajor;
        }

        public int Major
        {
            get { return _major; }
        }

        public int Minor
        {
            get { return _minor; }
        }

        public int RawValue
        {
            get { return _major * MinorsPerMajor + _minor; }
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
            return $"€ {Major:D1},{Minor:D2}";
        }
    }
}