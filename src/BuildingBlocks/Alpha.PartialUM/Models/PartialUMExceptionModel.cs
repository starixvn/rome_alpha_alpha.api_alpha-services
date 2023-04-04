namespace Alpha.PartialUM.Models;

public sealed class PartialUMExceptionModel
{
    public PartialUMExceptionModel(string umFrom, string umTo)
    {
        UMFrom = umFrom;
        UMTo = umTo;
    }

    public string UMFrom { get; }

    public string UMTo { get; }

    public string Message => "{\"UMFrom\":\"" + UMFrom + "\",\"UMTo\":\"" + UMTo + "\"}";
}