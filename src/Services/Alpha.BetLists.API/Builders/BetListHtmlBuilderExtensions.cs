using Alpha.BetLists.API.Builders.PTCommBuilders;
using Alpha.BetLists.API.Models;
using Alpha.BetLists.API.Templates;
using Fanex.BetList.Core.Entities;

namespace Alpha.BetLists.API.Builders;

public static class BetListHtmlBuilderExtensions
{
    public static PTComm_Template BuildPtComm(ITicket ticket, List<ITicketData> ticketData, UserLevel userLevel)
    {
        var template = new PTComm_Template();
        var positionTakingComm = PTCommBaseBuilderFactory.CreateInstance(ticket, ticketData, userLevel);

        RenderPtPercent(template, ticket, positionTakingComm);
        RenderPtValue(template, positionTakingComm);
        RenderCommPercent(template, positionTakingComm);
        RenderCommValue(template, positionTakingComm);

        return template;
    }

    private static void RenderPtPercent(PTComm_Template template, ITicket ticket, PTCommBuilderBase positionTakingComm)
    {
        if (ticket.BetTypeId == 33 || ticket.BetTypeId == 1233)
        {
            RenderRacingPtValue(template, positionTakingComm);
            return;
        }

        if (!string.IsNullOrEmpty(positionTakingComm.PositionTakingPercent) && positionTakingComm.VisibilePositionTakingPercent)
        {
            template.PT.PT_Percent = positionTakingComm.PositionTakingPercent;
        }
        else
        {
            template.PT.Hide();
        }
    }

    private static void RenderPtValue(PTComm_Template template, PTCommBuilderBase positionTakingComm)
    {
        if (!string.IsNullOrEmpty(positionTakingComm.PositionTaking) && positionTakingComm.VisiblePositionTaking)
        {
            template.PTValue.PT_Value = positionTakingComm.PositionTaking;
        }
        else
        {
            template.PTValue.Hide();
        }
    }

    private static void RenderCommPercent(PTComm_Template template, PTCommBuilderBase positionTakingComm)
    {
        if (!string.IsNullOrEmpty(positionTakingComm.CommissionPercent) && positionTakingComm.VisibileCommisionPercent)
        {
            template.Comm.Comm_Percent = positionTakingComm.CommissionPercent;
        }
        else
        {
            template.Comm.Hide();
        }
    }

    private static void RenderCommValue(PTComm_Template template, PTCommBuilderBase positionTakingComm)
    {
        if (!string.IsNullOrEmpty(positionTakingComm.Commission) && positionTakingComm.VisibleCommision)
        {
            template.CommValue.Comm_value = positionTakingComm.Commission;
        }
        else
        {
            template.CommValue.Hide();
        }
    }

    private static void RenderRacingPtValue(PTComm_Template template, PTCommBuilderBase positionTakingComm)
    {
        var positionTakingComm33 = positionTakingComm as PTCommBuilder33;
        var racingTemplate = new PTRacing_Template();
        racingTemplate.PT_Win = positionTakingComm33.PositionTakingWin;
        racingTemplate.PT_Place = positionTakingComm33.PositionTakingPlace;
        template.PT.PT_Percent = racingTemplate;
    }
}