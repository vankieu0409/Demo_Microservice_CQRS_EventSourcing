using Iot.Class.Domain.Dtos;
using Iot.Class.Domain.ReadModels;
using MediatR;

namespace Iot.Class.Infrastructure.Queries;

public record GetClassAllQuery : IRequest<IQueryable<ClassDto>>
{
    public GetClassAllQuery()
    {

    }
}