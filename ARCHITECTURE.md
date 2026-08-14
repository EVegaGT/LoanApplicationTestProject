# Architecture & Design Decisions

This solution follows **Clean Architecture** and **Domain-Driven Design (DDD)** principles.

## 1. Project Structure

*   **`Domain`**: The core of the business. Contains entities (`Customer`, `Application`), the contracts (`ILoanDecisionRule`), pure business rules (e.g., `NyStateDenyRule`), and the `LoanDecisionDomainService` (The Rule Engine). It has zero external dependencies.
*   **`Application`**: Orchestrates business use cases (e.g., `ProcessLoanApplicationUseCase`). It acts as a manager: fetches data from repositories, delegates the decision to the Domain's Rule Engine, and returns a `Result` object to avoid using exceptions for control flow.
*   **`Infrastructure`**: Implements interfaces defined in the Domain. Contains the EF Core `DbContext`, the Typed HTTP Client for the external mock, the Outbox Background Worker, and "impure" rules that require database access (e.g., `BlacklistedSsnRule`).
*   **`API`**: The presentation layer. Controllers/Minimal APIs act as thin wrappers around Application use cases, transforming the `Result` patterns into appropriate HTTP responses (e.g., HTTP 422 with a redirect URL for denied applications).
*   **`MockExternalService`**: A completely independent, lightweight .NET Minimal API acting as the external webhook receiver.

## 2. The Rule Engine (Open/Closed Principle)

The decision logic is decoupled using the **Rules Engine Pattern**

*   **How it works**: Rules implement the `ILoanDecisionRule` interface and return a `DecisionResult`. The `LoanDecisionDomainService` receives an `IEnumerable<ILoanDecisionRule>` via Dependency Injection. It evaluates them sequentially and short-circuits if any rule denies the application.
*   **How to add a new rule**: Simply create a new class implementing `ILoanDecisionRule` (e.g., `DenyLowAmountRule`). Thanks to reflection configured in the DI container, the system automatically scans the assemblies, registers the new rule, and injects it into the engine. **Zero modifications** to existing rules, the engine, or the Application layer are required.

## 3. Transaction Handling & Background Events (The Outbox Pattern)

The requirement states that saving the customer, the application, and publishing the event must be a single, atomic unit of work without delaying the HTTP response. To avoid the "Dual Write" problem and guarantee delivery, I implemented the **Transactional Outbox Pattern**.

1.  **The Transaction**: When a valid form is submitted, the `Customer`, `Application`, and an `OutboxMessage` (containing the JSON payload) are written to the database in a single `SaveChangesAsync()` call. If the database fails, the entire transaction rolls back atomically.
2.  **The Background Event**: A .NET `BackgroundService` polls the `OutboxMessages` table for pending records.
3.  **The External Call**: The worker uses a Typed `HttpClient` to send the payload to the Mock Service. 
4.  **Resiliency & DLQ**: The user gets an immediate response from the API. If the external mock service is down or rejects the payload, the background worker catches the failure, increments a retry counter, and tries again in the next cycle. If the message exceeds the maximum allowed retries, it is flagged as a "Dead Letter" to prevent infinite loops and poison messages from blocking the queue.

## 4. Trade-offs & Simplifications

To adhere to the "simplicity" requirement, the following trade-offs were made:
*   **Database Polling vs. Message Broker**: Chosen to use a simple polling background worker for the Outbox instead of introducing a full message broker (like RabbitMQ or Kafka) or Change Data Capture (CDC). This guarantees zero-setup local execution for the reviewer while still proving the distributed systems concept.
*   **Idempotent Mock Webhook**: The external mock service uses a single `/api/LoanApplication` endpoint that handles the synchronization based on the SSN/CustomerId. This simplifies the payload and ensures idempotency if the Outbox worker accidentally retries a message due to a network timeout.
*   **State Management (Frontend)**: No Redux or Zustand. The application is a linear flow, making React local state (`useState`) and Zod schema validation (via `react-hook-form`) the most efficient and readable approach.