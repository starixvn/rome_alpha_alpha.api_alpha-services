namespace Alpha.BetLists.API.Models;

public class BetlistOutputParams
{
    public BetlistOutputParams()
    {
    }

    public BetlistOutputParams(int errCode, string errMsg, int maxTotalRow)
    {
        ErrCode = errCode;
        ErrMsg = errMsg;
        MaxTotalRow = maxTotalRow;
    }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public int MaxTotalRow { get; set; }
}