import * as z from "zod";

// we use zod to define the schema for our loan application form. 
// This schema will be used for validation in the frontend and can also be used in the backend to ensure consistency.
export const loanApplicationSchema = z.object({
  firstName: z.string().min(2, "First name is required"),
  lastName: z.string().min(2, "Last name is required"),
  ssn: z.string().regex(/^\d{9}$/, "SSN must be exactly 9 digits"),
  address: z.string().min(5, "Address is required"),
  state: z.string().length(2, "State must be a 2-letter abbreviation (e.g., CA)"),
  companyName: z.string().min(2, "Company name is required"),
  requestedAmount: z.coerce.number().min(100, "Amount must be at least $100"),
});

export type LoanApplicationFormValues = z.infer<typeof loanApplicationSchema>;