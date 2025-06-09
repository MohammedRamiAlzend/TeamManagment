using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using TMS.Core;
using TMS.Core.AutoMapperClasses.DTOs;
using TMS.Core.Entities.Interfaces;
using TMS.Core.MediatR.Interfaces;

namespace Contracts.Requests.GenericRequests;

public record GetEntityQuery<TEntity, TEntityDto>(
    Expression<Func<TEntity, bool>>? Filter = null,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? Include = null
) : IRequest<ApiResponse<TEntityDto>>
    where TEntity : Entity
    where TEntityDto : IDto;