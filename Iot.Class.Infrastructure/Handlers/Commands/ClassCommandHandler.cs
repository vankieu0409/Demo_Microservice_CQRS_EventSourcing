﻿using System.Text.Json;
using AutoMapper;

using DnsClient.Internal;

using Iot.Class.Domain.AggregateRoot;
using Iot.Class.Domain.Dtos;
using Iot.Class.Infrastructure.Commands.Classes;
using Iot.Core.EventStore.Abstraction.Interfaces;
using Iot.Core.Extensions;
using Iot.Core.Infrastructure.Exceptions;

using MediatR;
using Microsoft.Extensions.Logging;

namespace Iot.Class.Infrastructure.Handlers.Commands;

public class ClassCommandHandler : IRequestHandler<CreateClassCommand, ClassDto>,
                                    IRequestHandler<UpdateClassCommand, ClassDto>,
                                    IRequestHandler<RemoveClassCommand, ClassDto>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IEventStoreService<ClassAggregateRoot> _eventStoreService;
    private readonly ILogger<ClassCommandHandler> _logger;

    public ClassCommandHandler(IMapper mapper, IMediator mediator, IEventStoreService<ClassAggregateRoot> eventStoreService, ILogger<ClassCommandHandler> logger)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _eventStoreService = eventStoreService ?? throw new ArgumentNullException(nameof(eventStoreService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    public async Task<ClassDto> Handle(CreateClassCommand request, CancellationToken cancellationToken)
    {
        _logger.LogDebug($"Command - {request.Id} - Start Handle {nameof(request)}");
        var aggregateRoot = new ClassAggregateRoot(request.Id);
        _logger.LogDebug($"Command - {request.Id} - Handle {nameof(request)} \n Payload: \n {JsonSerializer.Serialize(aggregateRoot)}");
        aggregateRoot.Initialize(request.ClassName, request.Teacher, request.PhoneNumber, request.Room);
        _logger.LogDebug($"Command - {request.Id} - Handle {nameof(request)} \n Payload: \n {JsonSerializer.Serialize(aggregateRoot)}");
        await _eventStoreService.StartStreamAsync(aggregateRoot.StreamName, aggregateRoot);
        _logger.LogDebug($"Command - {request.Id} - End Handle {nameof(request)}");
        return _mapper.Map<ClassDto>(aggregateRoot);
    }

    public async Task<ClassDto> Handle(UpdateClassCommand request, CancellationToken cancellationToken)
    {
        var aggregateRoot = new ClassAggregateRoot(request.Id);

        var aggregateRootStream = await _eventStoreService.AggregateStreamAsync(aggregateRoot.StreamName);
        if (aggregateRootStream.IsNullOrDefault())
            throw new EntityNotFoundException().WithData("Class Id: " + request.Id);

        if (!string.Equals(request.ClassName, aggregateRootStream.ClassName))
            aggregateRootStream.ChangeName(request.ClassName);


        if (!string.Equals(request.Teacher, aggregateRootStream.Teacher))
            aggregateRootStream.ChangeTeacher(request.Teacher);

        if (!string.Equals(request.PhoneNumber, aggregateRootStream.PhoneNumber))
            aggregateRootStream.ChangePhoneNumber(request.PhoneNumber);

        if (!int.Equals(request.Room, aggregateRootStream.Room))
            aggregateRootStream.ChangeRoom(request.Room);


        await _eventStoreService.AppendStreamAsync(aggregateRoot.StreamName, aggregateRootStream);
        return _mapper.Map<ClassDto>(aggregateRootStream);
    }

    public async Task<ClassDto> Handle(RemoveClassCommand request, CancellationToken cancellationToken)
    {
        var aggregateRoot = new ClassAggregateRoot(request.Id);
        var aggregateRootStream = await _eventStoreService.AggregateStreamAsync(aggregateRoot.StreamName);
        if (aggregateRootStream.IsNullOrDefault() && aggregateRootStream.IsDeleted)
            throw new EntityNotFoundException().WithData("Class Id: " + request.Id);

        if (Boolean.Equals(aggregateRootStream.IsDeleted, false))
        {
            aggregateRootStream.Delete(aggregateRootStream.Id);
            await _eventStoreService.AppendStreamAsync(aggregateRoot.StreamName, aggregateRootStream);
        }

        return _mapper.Map<ClassDto>(aggregateRootStream);
    }
}