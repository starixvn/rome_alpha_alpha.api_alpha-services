namespace Ext.ApiGateways.Models;

public record CustomerStatus(
    int CustId,
    int SubAccId,
    bool IsActive,
    bool IsProtectedMode,
    bool IsLocked,
    bool IsSubAcc,
    bool HasReportPermission,
    bool HasMemberInformationPermission);