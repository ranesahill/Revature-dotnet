using System.Drawing;

public abstract class Fruit
{
    public double Price { get; set; }
    protected Fruit()
    {
        Price = 100;
    }

    // Pure abstract class
    // public abstract void ApplyDiscount();

    public virtual void ApplyDiscount()
    {
        Price = Price * .9;

        Console.WriteLine($"Price after base discount: {Price}");
    }
}


public class Apple : Fruit
{
    public new void ApplyDiscount()
    {
        base.ApplyDiscount();

        Price = Price * .75;

        Console.WriteLine($"Price after chrimas discount: {Price}");
    }
}

public sealed class GreenApple : Apple
{
    public new void ApplyDiscount()
    {
        Console.WriteLine($"Green Apples Price {Price}");
    }
}

// Sealed class inheritance is not allowed
//public class GreenerApple : GreenApple
//{ 
//}