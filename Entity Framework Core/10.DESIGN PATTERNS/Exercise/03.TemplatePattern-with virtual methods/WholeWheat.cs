namespace P03.TemplatePattern
{
    using System;

    public class WholeWheat : Bread
    {
        public override void Bake()
        {
            base.Bake();
            Console.WriteLine("(15 minutes)");
        }

        public override void MixIngredients()
        {
            Console.WriteLine($"Gathering Ingredients for Whole Wheat Bread.");
        }
    }
}
