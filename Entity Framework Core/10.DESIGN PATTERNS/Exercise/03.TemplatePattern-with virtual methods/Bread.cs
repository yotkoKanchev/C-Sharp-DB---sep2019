namespace P03.TemplatePattern
{
    using System;

    public abstract class Bread
    {
        public virtual void MixIngredients()
        {
            Console.WriteLine($"Gathering Ingredients for {this.GetType().Name} Bread.");
        }

        public virtual void Bake()
        {
            Console.Write($"Baking the {this.GetType().Name} Bread.");
        }

        public virtual void Slice()
        {
            Console.WriteLine("Slicing the " + GetType().Name + " bread!");
        }

        public void Make()
        {
            this.MixIngredients();
            this.Bake();
            this.Slice();
        }
    }
}
