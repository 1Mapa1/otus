using BillingService.Application.Accounts.CreateAccount;
using BillingService.Infrastructure.Messaging.Contracts;
using MediatR;
using System.Text.Json;

namespace BillingService.Infrastructure.Messaging.Handlers
{
    internal sealed class UserActivatedIntegrationEventHandler
        : IIntegrationEventHandler
    {
        private static readonly JsonSerializerOptions JsonOptions =
            new(JsonSerializerDefaults.Web)
            {
                PropertyNameCaseInsensitive = true
            };

        private readonly ISender _sender;

        public UserActivatedIntegrationEventHandler(ISender sender)
        {
            _sender = sender;
        }

        public string EventType => "user.activated.v1";

        public Task HandleAsync(
            IntegrationEventEnvelope message,
            CancellationToken cancellationToken)
        {
            UserActivatedV1? @event;

            try
            {
                @event = message.Data.Deserialize<UserActivatedV1>(JsonOptions);
            }
            catch (JsonException ex)
            {
                throw new DeadLetterMessageException(
                    $"Payload for {EventType} is invalid.",
                    ex);
            }

            if (@event is null)
            {
                throw new DeadLetterMessageException(
                    $"Payload for {EventType} is invalid.");
            }

            return _sender.Send(
                new CreateAccountCommand(@event.UserId),
                cancellationToken);
        }
    }
}
