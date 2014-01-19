namespace FeesCalculator.ConsoleApplication.Configuration
{
    public enum AdapterFactory
    {
        JsonFeesCalc =1,
        MtbBsClnt = 2,
        BsbHtmlExportCsvImport =3,
        //This adapter was added by Vladimir Makaev for his client bank
        BelSwissClient = 4
    }
}