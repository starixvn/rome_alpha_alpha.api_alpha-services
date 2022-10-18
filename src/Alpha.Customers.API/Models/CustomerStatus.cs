namespace Alpha.Customers.API.Models;
/*
    [A]: Create
    [B]: Update
    [C]: Transfer
    [D]: Total Bets
    [E]: Reports
    [F]: Forcast
    [H]: Member Information
    [G]: View Log
    [L]: Bet Lists
*/
public record CustomerStatus(
    int CustId,
    int SubAccId,
    bool IsActive,
    bool IsProtectedMode,
    bool IsLocked,
    string PageAccess)
{
    private const string Reports = "[E]";
    private const string MemberInformation = "[H]";

    public bool IsSubAcc => SubAccId != 0;

    public bool HasReportPermission => !IsSubAcc || PageAccess.Contains(Reports);

    public bool HasMemberInformationPermission => !IsSubAcc || PageAccess.Contains(MemberInformation);
}