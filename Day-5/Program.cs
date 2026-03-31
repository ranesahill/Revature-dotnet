class Animal
{
    public virtual void sound()
    {
      Console.WriteLine("Animal makes sound");  
    }
   
}
class Dog : Animal
{
    public override void sound()
    {
        Console.WriteLine("Dog barks");
    }
}
class Program
{
         static void Main()
    {
        Animal a = new Dog();
        a.sound();
    }

}

