
using System.Data;
using Alpha.Reports.API.Criteria;
using Alpha.Reports.API.Models;
using Alpha.Reports.API.Models.WinLossDetail;
using Fanex.Common.Extensions;

namespace Alpha.Reports.API.DataAccesses;

public interface IWinLossDetailDataAccess
{
    Task<PagedReportDataModel<SuperWinLossDetailModel>> GetSuperWinLossDetail(GetWinLossDetailCriteria criteria);
    Task<PagedReportDataModel<MasterWinLossDetailModel>> GetMasterWinLossDetail(GetWinLossDetailCriteria criteria);
    Task<PagedReportDataModel<AgentWinLossDetailModel>> GetAgentWinLossDetail(GetWinLossDetailCriteria criteria);
}

public class WinLossDetailDataAccess : IWinLossDetailDataAccess
{
    public async Task<PagedReportDataModel<SuperWinLossDetailModel>> GetSuperWinLossDetail(GetWinLossDetailCriteria criteria)
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
                    winLossDetails.Add(BuildSuperWinLossDetail(reader));
                }

                if (reader.NextResult() && reader.Read())
                {
                    totalWinLoss = BuildSuperWinLossDetail(reader);
                }
            }
        }

        return new PagedReportDataModel<SuperWinLossDetailModel>(
            winLossDetails,
            criteria.PageIndex,
            criteria.PageSize,
            criteria.TotalRecords,
            totalWinLoss);
    }

    public async Task<PagedReportDataModel<MasterWinLossDetailModel>> GetMasterWinLossDetail(GetWinLossDetailCriteria criteria)
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
                    winLossDetails.Add(BuildMasterWinLossDetail(reader));
                }

                if (reader.NextResult() && reader.Read())
                {
                    totalWinLoss = BuildMasterWinLossDetail(reader);
                }
            }
        }

        return new PagedReportDataModel<MasterWinLossDetailModel>(
            winLossDetails,
            criteria.PageIndex,
            criteria.PageSize,
            criteria.TotalRecords,
            totalWinLoss);
    }


    public async Task<PagedReportDataModel<AgentWinLossDetailModel>> GetAgentWinLossDetail(GetWinLossDetailCriteria criteria)
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

        return new PagedReportDataModel<AgentWinLossDetailModel>(
            winLossDetails,
            criteria.PageIndex,
            criteria.PageSize,
            criteria.TotalRecords,
            totalWinLoss);
    }

    private static SuperWinLossDetailModel BuildSuperWinLossDetail(IDataReader reader)
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
            SuperComm = reader.ToDecimal("SAgComm")
        };

        return winLossDetail;
    }

    private static MasterWinLossDetailModel BuildMasterWinLossDetail(IDataReader reader)
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
            MasterComm = reader.ToDecimal("MAgComm")
        };

        return winLossDetail;
    }

    private static AgentWinLossDetailModel BuildAgentWinLossDetail(IDataReader reader)
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
            AgentComm = reader.ToDecimal("AgComm")
        };

        return winLossDetail;
    }
}