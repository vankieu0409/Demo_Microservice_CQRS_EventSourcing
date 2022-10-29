using Iot.Class.Domain.Dtos;
using Iot.Class.Domain.ReadModels;

using MediatR;

namespace Iot.Class.Infrastructure.Queries;

public class GetClassByIdQuery : IRequest<ClassDto>
{
    public GetClassByIdQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}