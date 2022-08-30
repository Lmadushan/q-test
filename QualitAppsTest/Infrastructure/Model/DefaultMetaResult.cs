using static QualitAppsTest.Common.Constants;

namespace QualitAppsTest.Infrastructure.Model
{
    public class DefaultMetaResult
    {
        public DefaultMetaResult()
        {
            ResponseCode = MetaResult.ResponseCode;
            ResponseMessage = MetaResult.ResponseDesc;
        }
        public DefaultMetaResult(int responseCode, string responseDesc)
        {
            ResponseCode = responseCode;
            ResponseMessage = responseDesc;
        }
        public int ResponseCode { get; set; }

        public string ResponseMessage { get; set; }
    }
}
