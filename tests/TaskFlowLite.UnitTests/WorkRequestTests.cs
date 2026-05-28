using TaskFlowLite.Domain.Entities;
using TaskFlowLite.Domain.Enums;

namespace TaskFlowLite.UnitTests;

public class WorkRequestTests
{
    [Fact]
    public void ChangeStatus_ToDone_SetsClosedAtUtc()
    {
        var request = new WorkRequest();

        request.ChangeStatus(WorkRequestStatus.Done);

        Assert.Equal(WorkRequestStatus.Done, request.Status);
        Assert.NotNull(request.ClosedAtUtc);
    }

    [Fact]
    public void ChangeStatus_FromDoneBackToInProgress_Throws()
    {
        var request = new WorkRequest();
        request.ChangeStatus(WorkRequestStatus.Done);

        Assert.Throws<InvalidOperationException>(() => request.ChangeStatus(WorkRequestStatus.InProgress));
    }
}
