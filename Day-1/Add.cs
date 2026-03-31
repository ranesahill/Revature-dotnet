

public class Add
{
    public int addition(int a,int b)
    {
        return a + b;
    }
    public double addition(double a, double b, double c)
    {
        return a + b + c;
    }

    public static void Main (String [] args) 
    {
        Add sub = new Add();
        int result1 = sub.addition(2,3);
        double result2 = sub.addition(3,4,5);
        Console.WriteLine("Addition of two nos:"+ result1);
         Console.WriteLine("Addition of three nos:"+ result2);
    }
}
