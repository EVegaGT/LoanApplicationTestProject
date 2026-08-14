# Loan Application Flow (Full-Stack .NET + Next.js)

**🎥 Link to Video Walkthrough Placeholder**  
*https://www.loom.com/share/e3feeaea793f41eda601a16362ef496f*

## 1. Test Data & Scenarios

To evaluate the rule engine and persistence paths, use the following data:

*   **Approve Application:** Use any valid SSN (e.g., `123456789`) and any State other than `NY` (e.g., `CA`).
*   **Deny Rule 1 (State):** Set the State field to `NY`.
*   **Deny Rule 2 (Blacklisted SSN):** Use SSN `000000000` or `999999999`, (e.g, `784096895`).
*   **Returning Customer (Update Path):** Submit an approved application. Then, submit a second application using the **exact same SSN**, but change the `Requested Amount` or `First Name`. The system will update the existing records instead of duplicating them.

## 2. How to Run Everything Locally

This solution is designed for simplicity. It uses SQLite with automatic database creation (`EnsureCreated`), meaning **no external Docker containers or manual EF migrations are required.**

### Prerequisites
*   .NET 10 SDK (or your target version)
*   Node.js (v18+)

### Step 1: Start the Backend & Mock Service
Open a terminal in the root of the solution and run the following command to start both the main API and the Mock Service simultaneously:

**For Mac/Linux (Bash/Zsh):**
```sh
dotnet run --project ExternalServiceProject/ExternalServiceProject.csproj --launch-profile https &
dotnet run --project LoanTestProject/LoanTestProject.csproj --launch-profile https
```

**For Windows PowerShell:**
```sh
Start-Process dotnet -ArgumentList "run --project ExternalServiceProject\ExternalServiceProject.csproj --launch-profile https";
dotnet run --project LoanTestProject\LoanTestProject.csproj --launch-profile https
```

Alternatively, you can run both projects using the shared launch profile configured in `LoanTestProject.slnLaunch`:

### Step 2: Start the Frondend app
Open a terminal in the root of the solution and run the following command 

```bash
cd loan-app-frontend
npm install
npm run dev
```

###  How to Run the Tests
Open a terminal in the root of the solution and run the following command 
```bash
dotnet test
```

### Test data: 
* SSNs are in blacklist: `"784096895", "987654321"`
* States denied: `NY`