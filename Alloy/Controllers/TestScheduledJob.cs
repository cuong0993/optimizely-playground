using EPiServer.PlugIn;
using EPiServer.Scheduler;

namespace Alloy.Controllers;

[ScheduledPlugIn(DisplayName = "Test job",
    GUID = "88E83CA0-92ED-46FD-A338-7938C8D0FDF9", SortIndex = -1)]
public class TestScheduledJob : ScheduledJobBase
{
    public TestScheduledJob()
    {
    }


    public override void Stop()
    {
    }

    public override string Execute()
    {
        return "Done";
    }
}