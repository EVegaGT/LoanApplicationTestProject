"use client"

import { Suspense } from "react"
import Link from "next/link"
import { useSearchParams } from "next/navigation"
import { XCircle } from "lucide-react"

import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"

function DeniedContent() {
  const searchParams = useSearchParams()
  const reason = searchParams.get("reason") || "Your application did not meet the required criteria."

  return (
    <Card className="w-full max-w-md shadow-sm border-t-4 border-t-destructive">
      <CardHeader className="text-center space-y-4">
        <div className="mx-auto bg-destructive/10 w-16 h-16 flex items-center justify-center rounded-full">
          <XCircle className="w-8 h-8 text-destructive" />
        </div>
        <CardTitle className="text-2xl font-bold tracking-tight">
          Application Denied
        </CardTitle>
        <CardDescription className="text-base">
          Unfortunately, we are unable to approve your application at this time.
        </CardDescription>
      </CardHeader>
      <CardContent className="text-center">
        <div className="p-4 bg-zinc-100 rounded-md text-sm font-medium text-zinc-800">
          {reason}
        </div>
      </CardContent>
      <CardFooter className="flex justify-center mt-4">
        <Button variant="outline" className="w-full">
          <Link href="/loan">Start New Application</Link>
        </Button>
      </CardFooter>
    </Card>
  )
}

export default function DeniedPage() {
  return (
    <main className="min-h-screen flex items-center justify-center p-4 bg-zinc-50">
      <Suspense fallback={<div className="text-sm text-zinc-500">Loading result...</div>}>
        <DeniedContent />
      </Suspense>
    </main>
  )
}