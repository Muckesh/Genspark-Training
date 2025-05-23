using System;

class Program
{
    static void Main(string[] args)
    {
        ShapeFactory factory = new ShapeFactory();

        IShape shape1 = factory.CreateShape("Circle");
        shape1.Draw();  // Output: Drawing a Circle

        IShape shape2 = factory.CreateShape("Square");
        shape2.Draw();  // Output: Drawing a Square

        IShape shape3 = factory.CreateShape("Triangle");
        shape3.Draw();  // Output: Drawing a Triangle
    }
}
