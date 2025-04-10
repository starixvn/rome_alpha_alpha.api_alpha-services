
using System.Data;
using Alpha.Reports.API.Criteria;
using Alpha.Reports.API.Models;
using Alpha.Reports.API.Models.WinLossDetail;
using Fanex.Common.Extensions;
using Fanex.Data.Repository;

namespace Alpha.Reports.API.DataAccesses;

public interface IWinLossDetailDataAccess
{
    Task<ReportDataModel<SuperWinLossDetailModel>> GetSuperWinLossDetail(GetWinLossDetailCriteria criteria);
    Task<ReportDataModel<MasterWinLossDetailModel>> GetMasterWinLossDetail(GetWinLossDetailCriteria criteria);
    Task<ReportDataModel<AgentWinLossDetailModel>> GetAgentWinLossDetail(GetMemberWinLossDetailCriteria criteria);
    Task<SummaryWinLossDetail> GetSuperWinLossDetailSummary(ICriteria criteria, string member);
    Task<SummaryWinLossDetail> GetMasterWinLossDetailSummary(ICriteria criteria, string member);
    Task<SummaryWinLossDetail> GetAgentWinLossDetailSummary(ICriteria criteria, string member);
}

public class WinLossDetailDataAccess : IWinLossDetailDataAccess
{
    public async Task<ReportDataModel<SuperWinLossDetailModel>> GetSuperWinLossDetail(GetWinLossDetailCriteria criteria)
    {
        var winLossDetails = new List<SuperWinLossDetailModel>();
        var totalWinLoss = new SuperWinLossDetailModel();

        if (criteria.IsValid())
        {
            using var db = ReportObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            using var reader = await db.ExecuteReaderAsync(criteria);
            if (reader != null)
            {
                while (reader.Read())
                {
                    var isDirectMember = reader.ToString("IsDirectMember") == "1";
                    winLossDetails.Add(BuildSuperWinLossDetail(reader, isDirectMember));
                }

                if (reader.NextResult() && reader.Read())
                {
                    totalWinLoss = BuildSuperWinLossDetail(reader, false);
                }
            }
        }

        return new ReportDataModel<SuperWinLossDetailModel>(winLossDetails, totalWinLoss);
    }

    public async Task<ReportDataModel<MasterWinLossDetailModel>> GetMasterWinLossDetail(GetWinLossDetailCriteria criteria)
    {
        var winLossDetails = new List<MasterWinLossDetailModel>();
        var totalWinLoss = new MasterWinLossDetailModel();

        if (criteria.IsValid())
        {
            using var db = ReportObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            using var reader = await db.ExecuteReaderAsync(criteria);
            if (reader != null)
            {
                while (reader.Read())
                {
                    var isDirectMember = reader.ToString("IsDirectMember") == "1";
                    winLossDetails.Add(BuildMasterWinLossDetail(reader, isDirectMember));
                }

                if (reader.NextResult() && reader.Read())
                {
                    totalWinLoss = BuildMasterWinLossDetail(reader, false);
                }
            }
        }

        return new ReportDataModel<MasterWinLossDetailModel>(winLossDetails,
            totalWinLoss);
    }

    public async Task<ReportDataModel<AgentWinLossDetailModel>> GetAgentWinLossDetail(GetMemberWinLossDetailCriteria criteria)
    {
        var winLossDetails = new List<AgentWinLossDetailModel>();
        var totalWinLoss = new AgentWinLossDetailModel();

        if (criteria.IsValid())
        {
            using var db = ReportObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            using var reader = await db.ExecuteReaderAsync(criteria);
            if (reader != null)
            {
                while (reader.Read())
                {
                    winLossDetails.Add(BuildAgentWinLossDetail(reader));
                }

                if (reader.NextResult() && reader.Read())
                {
                    totalWinLoss = BuildAgentWinLossDetail(reader);
                }
            }
        }

        return new ReportDataModel<AgentWinLossDetailModel>(winLossDetails, totalWinLoss);
    }

    private static SuperWinLossDetailModel BuildSuperWinLossDetail(IDataReader reader, bool isDirectMember)
    {
        var winLossDetail = new SuperWinLossDetailModel
        {
            CustId = reader.ToInt("CustId"),
            CustName = reader.ToString("CustName"),
            Turnover = reader.ToDecimal("Turnover"),
            BetCount = reader.ToInt("NumberOfTickets"),
            MemberWinLoss = reader.ToDecimal("MemberWinLoss"),
            MemberComm = reader.ToDecimal("MemberComm"),
            AgentWinLoss = reader.ToDecimal("AgWinLoss"),
            AgentComm = reader.ToDecimal("AgComm"),
            MasterWinLoss = reader.ToDecimal("MAgWinLoss"),
            MasterComm = reader.ToDecimal("MAgComm"),
            SuperWinLoss = reader.ToDecimal("SAgWinLoss"),
            SuperComm = reader.ToDecimal("SAgComm"),
            NetTurnover = reader.ToDecimal("TurnoverNoDraw"),
            GrossComm = reader.ToDecimal("GrossComm"),
            RewardAmount = reader.ToDecimal("RewardAmount"),
            RewardCurrency = reader.ToString("RewardCurrency"),
            IsDirectMember = isDirectMember
        };

        return winLossDetail;
    }

    private static MasterWinLossDetailModel BuildMasterWinLossDetail(IDataReader reader, bool isDirectMember)
    {
        var winLossDetail = new MasterWinLossDetailModel
        {
            CustId = reader.ToInt("CustId"),
            CustName = reader.ToString("CustName"),
            Turnover = reader.ToDecimal("Turnover"),
            BetCount = reader.ToInt("NumberOfTickets"),
            MemberWinLoss = reader.ToDecimal("MemberWinLoss"),
            MemberComm = reader.ToDecimal("MemberComm"),
            AgentWinLoss = reader.ToDecimal("AgWinLoss"),
            AgentComm = reader.ToDecimal("AgComm"),
            MasterWinLoss = reader.ToDecimal("MAgWinLoss"),
            MasterComm = reader.ToDecimal("MAgComm"),
            NetTurnover = reader.ToDecimal("TurnoverNoDraw"),
            GrossComm = reader.ToDecimal("GrossComm"),
            RewardAmount = reader.ToDecimal("RewardAmount"),
            RewardCurrency = reader.ToString("RewardCurrency"),
            IsDirectMember = isDirectMember
        };

        return winLossDetail;
    }

    private static AgentWinLossDetailModel BuildAgentWinLossDetail(IDataReader reader)
    {
        var winLossDetail = new AgentWinLossDetailModel
        {
            CustId = reader.ToInt("CustId"),
            CustName = reader.ToString("CustName"),
            Turnover = reader.ToDecimal("Turnover"),
            BetCount = reader.ToInt("NumberOfTickets"),
            MemberWinLoss = reader.ToDecimal("MemberWinLoss"),
            MemberComm = reader.ToDecimal("MemberComm"),
            AgentWinLoss = reader.ToDecimal("AgWinLoss"),
            AgentComm = reader.ToDecimal("AgComm"),
            NetTurnover = reader.ToDecimal("TurnoverNoDraw"),
            GrossComm = reader.ToDecimal("GrossComm"),
            RewardAmount = reader.ToDecimal("RewardAmount"),
            RewardCurrency = reader.ToString("RewardCurrency")
        };

        return winLossDetail;
    }

    public async Task<SummaryWinLossDetail> GetSuperWinLossDetailSummary(ICriteria criteria, string member)
    {
        if (criteria.IsValid())
        {
            using var db = ReportObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            using var reader = await db.ExecuteReaderAsync(criteria);
            if (reader != null)
            {
                if (string.IsNullOrWhiteSpace(member) && reader.NextResult() && reader.Read())
                {
                    var total = BuildSuperWinLossDetail(reader, false);
                    return new SummaryWinLossDetail(total);
                }

                while (reader.Read())
                {
                    var custName = reader.ToString("CustName");

                    if (custName.Equals(member, StringComparison.OrdinalIgnoreCase))
                    {
                        var total = BuildSuperWinLossDetail(reader, true);
                        return new SummaryWinLossDetail(total);
                    }
                }
            }
        }

        return null;
    }

    public async Task<SummaryWinLossDetail> GetMasterWinLossDetailSummary(ICriteria criteria, string member)
    {
        if (criteria.IsValid())
        {
            using var db = ReportObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            using var reader = await db.ExecuteReaderAsync(criteria);
            if (reader != null)
            {
                if (string.IsNullOrWhiteSpace(member) && reader.NextResult() && reader.Read())
                {
                    var total = BuildMasterWinLossDetail(reader, false);
                    return new SummaryWinLossDetail(total);
                }

                while (reader.Read())
                {
                    var custName = reader.ToString("CustName");

                    if (custName.Equals(member, StringComparison.OrdinalIgnoreCase))
                    {
                        var total = BuildMasterWinLossDetail(reader, true);
                        return new SummaryWinLossDetail(total);
                    }
                }
            }
        }

        return null;
    }

    public async Task<SummaryWinLossDetail> GetAgentWinLossDetailSummary(ICriteria criteria, string member)
    {
        if (criteria.IsValid())
        {
            using var db = ReportObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            using var reader = await db.ExecuteReaderAsync(criteria);
            if (reader != null)
            {
                if (string.IsNullOrWhiteSpace(member) && reader.NextResult() && reader.Read())
                {
                    var total = BuildAgentWinLossDetail(reader);
                    return new SummaryWinLossDetail(total);
                }

                while (reader.Read())
                {
                    var custName = reader.ToString("CustName");

                    if (custName.Equals(member, StringComparison.OrdinalIgnoreCase))
                    {
                        var total = BuildAgentWinLossDetail(reader);
                        return new SummaryWinLossDetail(total);
                    }
                }
            }
        }

        return null;
    }
}