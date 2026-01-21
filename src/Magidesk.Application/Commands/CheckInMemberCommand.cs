using System;
using MediatR;

namespace Magidesk.Application.Commands;

public class CheckInMemberCommand : IRequest<CheckInMemberResult>
{
    public Guid MemberId { get; set; }
}

public class CheckInMemberResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? VisitId { get; set; }
}
