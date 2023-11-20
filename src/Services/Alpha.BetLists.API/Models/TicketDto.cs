
using Fanex.BetList.Core.Entities;

namespace Alpha.BetLists.API.Models;

public class TicketDto : Ticket
{
    public int? DeviceType { get; set; }

    public string Platform => PlatformBuilderBase.BuildPlatform(DeviceType);
}