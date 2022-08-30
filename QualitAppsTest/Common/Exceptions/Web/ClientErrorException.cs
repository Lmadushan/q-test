using System.Collections;

namespace QualitAppsTest.Common.Exceptions.Web
{
    public class ClientErrorException : WebException
    {
        Dictionary<string, object> _data;
        public ClientErrorException(string msg, Dictionary<string, object> data) : base(msg)
        {
            _data = data;
        }
        public override IDictionary Data => _data;
        public override string Message => base.Message;
    }
}
