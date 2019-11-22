namespace P03.TemplatePattern
{
    using System;

    public class Sourdough : Bread
    {
        public override void Bake()
        {
            base.Bake();
            Console.WriteLine("(20 minutes)");
        }
    }
}
