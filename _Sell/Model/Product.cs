namespace _Sell.Model
{
    public class Product
    {
        private int id;
        private string _displayName;

        public Product(int id, string name, Price price, string displayName = null)
        {
            this.id = id;
            _displayName = displayName;
            this.Name = name;
            this.Price = price;
        }

        public int Id
        {
            get { return id; }
        }

        public string Name { get; set; }
        public Price Price { get; set; }

        public string DisplayName
        {
            get { return _displayName == null ? Name : _displayName; }
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Price);
        }
    }
}