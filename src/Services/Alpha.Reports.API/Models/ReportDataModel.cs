
namespace Alpha.Reports.API.Models;

public class ReportDataModel<T> where T : class
{
    public ReportDataModel(IEnumerable<T> dataSource, T totalData)
    {
        DataSource = dataSource ?? Enumerable.Empty<T>();
        TotalData = totalData;
    }

    public IEnumerable<T> DataSource { get; }

    public T TotalData { get; }

    public virtual bool Any() => DataSource.Any();

    public virtual bool MoreThanOne() => DataSource.Count() > 1;
}