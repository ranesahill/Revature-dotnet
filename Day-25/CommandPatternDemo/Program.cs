using System;

// Receiver
public class Light
{
    public void On()
    {
        Console.WriteLine("Light is ON");
    }

    public void Off()
    {
        Console.WriteLine("Light is OFF");
    }
}

// Command Interface
public interface ICommand
{
    void Execute();
}

// Concrete Command
public class LightOnCommand : ICommand
{
    private readonly Light _light;

    public LightOnCommand(Light light)
    {
        _light = light;
    }

    public void Execute()
    {
        _light.On();
    }
}

// Invoker
public class RemoteControl
{
    private ICommand _command;

    public RemoteControl(ICommand command)
    {
        _command = command;
    }

    public void PressButton()
    {
        _command.Execute();
    }
}

class Program
{
    static void Main()
    {
        Light light = new Light();

        ICommand command = new LightOnCommand(light);

        RemoteControl remote = new RemoteControl(command);

        remote.PressButton();
    }
}