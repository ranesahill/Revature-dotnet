public class PropertiesDemo
{
    readonly string name;

    public int MyProperty { get; init; }

    public int MyAge { get; private set; }

    public PropertiesDemo()
    {
        name = "Zia";
        MyProperty = 42;
        MyAge = 18;
    }

    public void ModifyName()
    {
        // name = "New Name"; // This will cause a compile-time error
        MyAge = 20;
    }
}