namespace QualitAppsTest.Infrastructure.Utils
{
    public interface ICommonResponse
    {
        int responseCode { get; set; }
        string responseMessage { get; set; }
    }
}
