namespace QualitAppsTest.Common.Exceptions.Web
{
    public class InvalidOperationException : WebException
    {
        public InvalidOperationException(string fmtText, params object[] args) :
            base(fmtText, args)
        {
        }
    }
}
