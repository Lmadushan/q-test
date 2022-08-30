namespace QualitAppsTest.Common.Exceptions.Web
{
    [Serializable]
    public class WebException : Exception
    {
        string _message;
        public string FormatText { get; private set; }
        protected WebException(string fmtText, params object[] args)
        {
            FormatText = fmtText;
            if (fmtText == null)
            {
                _message = String.Empty;
            }
            else
            {
                if (args == null || args.Length == 0)
                {
                    _message = fmtText;
                }
                else
                {
                    _message = String.Format(fmtText, args);
                }
            }
        }

        public override string Message => _message;
    }
}
