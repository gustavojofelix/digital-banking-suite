using MediatR;

namespace BankingSuite.BuildingBlocks.Application.CQRS;

public interface ICommand<out TResponse> : IRequest<TResponse>;

public interface IQuery<out TResponse> : IRequest<TResponse>;
