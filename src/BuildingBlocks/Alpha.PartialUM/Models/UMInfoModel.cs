namespace Alpha.PartialUM.Models;

public sealed class UMInfoModel
{
    public UMInfoModel() { }

    public UMInfoModel(
        string functionTypeName,
        DateTime beginTime,
        DateTime endTime)
    {
        FunctionTypeName = functionTypeName;
        BeginTime = beginTime;
        EndTime = endTime;
    }

    public string FunctionTypeName { get; }

    public DateTime BeginTime { get; }

    public DateTime EndTime { get; }
}