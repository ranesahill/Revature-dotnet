interface IElectricityProvider
{
    void SupplyService();
}

interface IInternetProvider
{
    void SupplyService();
}

public class Suppliers : IElectricityProvider, IInternetProvider
{
    void IElectricityProvider.SupplyService()
    {
        Console.WriteLine("Supplying electricity service.");
    }

    void IInternetProvider.SupplyService()
    {
        Console.WriteLine("Supplying intenret service.");
    }
}


public class ElectricityOnlySupplier : IElectricityProvider
{
    public void SupplyService()
    {
        Console.WriteLine("Supplying only electricity service.");
    }
}