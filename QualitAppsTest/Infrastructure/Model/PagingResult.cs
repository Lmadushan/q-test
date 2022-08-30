namespace QualitAppsTest.Infrastructure.Model;

public class PagingResult<T>
{
    public int RecordCount { get; set; }
    public IEnumerable<T> RecordList { get; set; }
}
