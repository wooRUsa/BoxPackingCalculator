namespace BoxPackingCalculator.App.ViewModels;

public sealed class PackingPrintPageViewModel
{
    public required IReadOnlyList<BoxResultGroupViewModel> Groups
    {
        get;
        init;
    }


    public PackingPrintSummaryViewModel? Summary
    {
        get;
        init;
    }


    public int PageNumber
    {
        get;
        init;
    }
    
    public string ShippingDestination
    {
        get;
        init;
    } = string.Empty;


    public string InvoiceNumber
    {
        get;
        init;
    } = string.Empty;


    public bool IsFirstPage =>
        PageNumber == 1;


    public bool HasSummary =>
        Summary is not null;
}