namespace P02.Facade
{
    public class CarBuilderFacade
    {
        public CarBuilderFacade()
        {
            this.Car = new Car();
        }

        protected Car Car { get; set; }

        public Car Build() => this.Car;

        public CarInfoBuilder Info => new CarInfoBuilder(Car);

        public CarAddressBuilder Built => new CarAddressBuilder(Car);
    }
}
