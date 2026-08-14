using Domain.EventsContracts;
using Domain.Interfaces;
using Infrastructure.BackgroundJobs.Payloads;
using Infrastructure.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundJobs
{
    public class OutboxLoanBackgroundService: BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OutboxLoanBackgroundService> _logger;
        private const int batchSize = 10; // Number of messages to process in each batch
        private const int MaxRetries = 3; // Maximum number of retry attempts before marking a message as dead letter

        public OutboxLoanBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<OutboxLoanBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Outbox Loan Processor is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxMessageRepository>();
                    var externalApiService = scope.ServiceProvider.GetRequiredService<IExternalApiService>();

                    var pendingMessages = await outboxRepository.GetPendingLoanOutboxMessagesAsync(batchSize);
                    foreach (var message in pendingMessages)
                    {
                        try
                        {
                            _logger.LogInformation("Processing Outbox Message: {MessageId}, EventType: {EventType}", message.Id, message.EventType);

                            // Deserialize the payload from the outbox message to the expected type
                            var payload = System.Text.Json.JsonSerializer.Deserialize<LoadRequestEventPayload>(message.Payload);
                            if (payload != null)
                            {
                                var isSuccess = await SendLoanRequestToExternalService(payload, externalApiService, stoppingToken);
                                if (isSuccess)
                                {
                                    message.Processed = true;
                                    message.ProcessedAt = DateTime.UtcNow;
                                    message.Error = null;
                                }
                                else
                                {
                                    message.RetryCount++;

                                    if (message.RetryCount >= MaxRetries)
                                    {
                                        _logger.LogCritical("Outbox Message {MessageId} reached max retries ({MaxRetries}) and is moved to Dead Letter.", message.Id, MaxRetries);

                                        message.IsDeadLetter = true; // Mark the message as dead letter after reaching max retries
                                        message.Error = $"DEAD LETTER (Max Retries Reached). Last API rejection.";
                                    }
                                    else
                                    {
                                        _logger.LogWarning("External service rejected Outbox Message {MessageId}. Attempt {Attempt} of {MaxRetries}", message.Id, message.RetryCount, MaxRetries);
                                        message.Error = $"Attempt {message.RetryCount} failed.";
                                    }
                                }
                            }
                            else
                            {
                                message.Processed = true;
                                message.Error = "Payload deserialization resulted in null.";
                            }
                            await outboxRepository.UpdateOutboxMessageAsync(message);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error processing Outbox Message: {message.Id}");
                            message.Error = ex.Message;
                            await outboxRepository.UpdateOutboxMessageAsync(message);
                        }
                    }
                }
                // Wait for a short period before checking for new messages again
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        private async Task<bool> SendLoanRequestToExternalService (LoadRequestEventPayload payload, IExternalApiService externalService, CancellationToken stoppingToken)
        {
            if (payload == null)
            {
                _logger.LogWarning("The request loan payload is null. Skipping sending to external service.");
                return false;
            }

            var externalPayload = new RequestLoanExternalPayload
            {
                Ssn = payload.Ssn,
                FirstName = payload.FirstName,
                LastName = payload.LastName,
                Address = payload.Address,
                State = payload.State,
                Amount = payload.RequestedAmount
            };

            var externalPayloadJson = System.Text.Json.JsonSerializer.Serialize(externalPayload);
            return await externalService.SyncLoanApplicationRequestAsync(externalPayloadJson, payload.IsNewCustomer, stoppingToken);
        }
    }
}
