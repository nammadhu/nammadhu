﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.DTOs;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Caching;
namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Commands.AddEdit;

public class AddEditVoteCommand: ICacheInvalidatorRequest<Result<int>>
{
      [Description("Id")]
      public int Id { get; set; }
          [Description("User Id")]
    public string? UserId {get;set;} 
    [Description("Constituency Id")]
    public int ConstituencyId {get;set;} 
    [Description("Constituency Id Delta")]
    public int? ConstituencyIdDelta {get;set;} 


      public string CacheKey => VoteCacheKey.GetAllCacheKey;
      public CancellationTokenSource? SharedExpiryTokenSource => VoteCacheKey.SharedExpiryTokenSource();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<VoteDto,AddEditVoteCommand>(MemberList.None);
            CreateMap<AddEditVoteCommand,Vote>(MemberList.None);
         
        }
    }
}

    public class AddEditVoteCommandHandler : IRequestHandler<AddEditVoteCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AddEditVoteCommandHandler> _localizer;
        public AddEditVoteCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<AddEditVoteCommandHandler> localizer,
            IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result<int>> Handle(AddEditVoteCommand request, CancellationToken cancellationToken)
        {
            if (request.Id > 0)
            {
                var item = await _context.Votes.FindAsync(new object[] { request.Id }, cancellationToken) ?? throw new NotFoundException($"Vote with id: [{request.Id}] not found.");
                item = _mapper.Map(request, item);
				// raise a update domain event
				item.AddDomainEvent(new VoteUpdatedEvent(item));
                await _context.SaveChangesAsync(cancellationToken);
                return await Result<int>.SuccessAsync(item.Id);
            }
            else
            {
                var item = _mapper.Map<Vote>(request);
                // raise a create domain event
				item.AddDomainEvent(new VoteCreatedEvent(item));
                _context.Votes.Add(item);
                await _context.SaveChangesAsync(cancellationToken);
                return await Result<int>.SuccessAsync(item.Id);
            }
           
        }
    }

