namespace P03.TemplatePattern
{
    using System;

    public class TwelveGrain : Bread
    {
        public override void Bake()
        {
            base.Bake();
            Console.WriteLine("(25 minutes)");
        }

        public override void MixIngredients()
        {
            Console.WriteLine($"Gathering Ingredients for 12-Grain Bread.");
        }
    }
}
