import { useState } from "react"
import { useRouter } from "next/navigation"
import type { LoanApplicationFormValues } from "@/schemas/loanApplicationSchema"

export function useLoanApplication() {
  const router = useRouter()
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [serverError, setServerError] = useState<string | null>(null)
  const externalApiUrl = process.env.NEXT_PUBLIC_API_URL;

  const submitApplication = async (data: LoanApplicationFormValues) => {
    setIsSubmitting(true)
    setServerError(null)

    try {
      const response = await fetch(`${externalApiUrl}/api/Loan/request`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data),
      })

      if (response.ok) {
        router.push("/success")
      } else if (response.status === 422) {
        // Handle validation errors
        const errorData = await response.json()
        const reason = encodeURIComponent(errorData.reason || "Application denied by rule engine.")
        router.push(`/denied?reason=${reason}`)
      } else {
        setServerError("An unexpected error occurred. Please try again later.")
      }
    } catch (error) {
      setServerError("Network error. Please ensure the server is running.")
    } finally {
      setIsSubmitting(false)
    }
  }

  return {
    submitApplication,
    isSubmitting,
    serverError,
  }
}