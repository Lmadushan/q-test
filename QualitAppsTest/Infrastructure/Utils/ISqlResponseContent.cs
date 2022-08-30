namespace QualitAppsTest.Infrastructure.Utils
{
    public interface ISqlResponseContent
    {
        string ResponseCode { get; set; }
        string ResponseMessage { get; set; }
    }
}
