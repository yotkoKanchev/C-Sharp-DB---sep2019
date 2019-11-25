namespace P02.Facade
{
    using System;
    public class Car
    {
        public string Type { get; set; }

        public string Color { get; set; }

        public int NumberOfDoors { get; set; }

        public string City { get; set; }

        public string Address { get; set; }

        public override string ToString()
        {
            return $"Car Type: {this.Type}, {Environment.NewLine}" +
                $"Color: {this.Color}, {Environment.NewLine}" +
                $"Number of doors: {this.NumberOfDoors}, {Environment.NewLine}" +
                $"Manufactured in {this.City}, {Environment.NewLine}" +
                $"at address: {this.Address}";
        }
    }
}
