import Link from "next/link"
import { CheckCircle2 } from "lucide-react"

import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"

export default function SuccessPage() {
  return (
    <main className="min-h-screen flex items-center justify-center p-4 bg-zinc-50">
      <Card className="w-full max-w-md shadow-sm border-t-4 border-t-emerald-500">
        <CardHeader className="text-center space-y-4">
          <div className="mx-auto bg-emerald-100 w-16 h-16 flex items-center justify-center rounded-full">
            <CheckCircle2 className="w-8 h-8 text-emerald-600" />
          </div>
          <CardTitle className="text-2xl font-bold tracking-tight">
            Application Sent Successfully
          </CardTitle>
          <CardDescription className="text-base">
            Congratulations! Your loan application has been successfully submitted.
          </CardDescription>
        </CardHeader>
        <CardContent className="text-center text-sm text-muted-foreground">
          Our team will be in touch shortly with the next steps. The application details have been securely synchronized with our external systems.
        </CardContent>
        <CardFooter className="flex justify-center mt-4">
          <Button className="w-full">
            <Link href="/">Return to Home</Link>
          </Button>
        </CardFooter>
      </Card>
    </main>
  )
}