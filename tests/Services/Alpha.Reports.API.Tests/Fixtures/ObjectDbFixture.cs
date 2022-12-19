using Alpha.Reports.API.DataAccesses;
using Fanex.Data;
using NSubstitute;

namespace Alpha.Reports.API.Tests.Fixtures;

public class ObjectDbFixture
{
    public ObjectDbFixture()
    {
        ObjectDb = Substitute.For<IObjectDb>();
        ReportObjectDbFactory.CreateInstance = (settingKey) => ObjectDb;
    }

    public IObjectDb ObjectDb { get; private set; }
}
