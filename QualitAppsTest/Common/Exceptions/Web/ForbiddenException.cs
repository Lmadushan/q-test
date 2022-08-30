namespace QualitAppsTest.Common.Exceptions.Web
{
    public class ForbiddenException : WebException
    {
        public ForbiddenException(string fmtText, params object[] args) :
            base(fmtText, args)
        {
        }
    }
}
