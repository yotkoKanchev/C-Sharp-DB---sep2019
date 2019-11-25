namespace P02.Facade
{
    using System;

    public class StartUp
    {
        public static void Main()
        {
            var car = new CarBuilderFacade()
                .Info
                    .WithType("Honda")
                    .WithColor("Silver")
                    .WithNumberOfDoors(5)
                .Built
                    .InCity("Hamamatsu")
                    .AtAddress("1 Chome-13-1 Aoihigashi, Naka Ward")
                .Build();

            Console.WriteLine(car);
        }
    }
}
