"use client"

import * as React from "react"
import { zodResolver } from "@hookform/resolvers/zod"
import { Controller, useForm } from "react-hook-form"

import { loanApplicationSchema, type LoanApplicationFormValues } from "@/schemas/loanApplicationSchema"
import { useLoanApplication } from "@/hooks/useLoanApplication"

import { Button } from "@/components/ui/button"
import {
    Card,
    CardContent,
    CardDescription,
    CardFooter,
    CardHeader,
    CardTitle,
} from "@/components/ui/card"
import {
    Field,
    FieldError,
    FieldGroup,
    FieldLabel,
} from "@/components/ui/field"
import { Input } from "@/components/ui/input"

export default function Home() {
    // we use the custom hook to handle the form submission and manage state
    const { submitApplication, isSubmitting, serverError } = useLoanApplication()

    const form = useForm<LoanApplicationFormValues>({
        resolver: zodResolver(loanApplicationSchema),
        defaultValues: {
            firstName: "",
            lastName: "",
            ssn: "",
            address: "",
            state: "",
            companyName: "",
            requestedAmount: 0,
        },
    })

    return (
        <main className="min-h-screen flex items-center justify-center p-4 bg-zinc-50">
            <Card className="w-full max-w-xl shadow-sm">
                <CardHeader>
                    <CardTitle className="text-2xl font-bold tracking-tight">Loan Application</CardTitle>
                    <CardDescription>
                        Please fill out the details below.
                    </CardDescription>
                </CardHeader>
                <CardContent>
                    {/* El handleSubmit delega la ejecución directamente al hook */}
                    <form id="loan-application-form" onSubmit={form.handleSubmit(submitApplication)}>
                        <FieldGroup className="space-y-4">

                            <div className="grid grid-cols-2 gap-4">
                                <Controller
                                    name="firstName"
                                    control={form.control}
                                    render={({ field, fieldState }) => (
                                        <Field data-invalid={fieldState.invalid}>
                                            <FieldLabel htmlFor="firstName">First Name</FieldLabel>
                                            <Input {...field} id="firstName" aria-invalid={fieldState.invalid} />
                                            {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
                                        </Field>
                                    )}
                                />
                                <Controller
                                    name="lastName"
                                    control={form.control}
                                    render={({ field, fieldState }) => (
                                        <Field data-invalid={fieldState.invalid}>
                                            <FieldLabel htmlFor="lastName">Last Name</FieldLabel>
                                            <Input {...field} id="lastName" aria-invalid={fieldState.invalid} />
                                            {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
                                        </Field>
                                    )}
                                />
                            </div>

                            <Controller
                                name="ssn"
                                control={form.control}
                                render={({ field, fieldState }) => (
                                    <Field data-invalid={fieldState.invalid}>
                                        <FieldLabel htmlFor="ssn">Social Security Number</FieldLabel>
                                        <Input {...field} id="ssn" maxLength={9} placeholder="000000000" aria-invalid={fieldState.invalid} />
                                        {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
                                    </Field>
                                )}
                            />

                            <div className="grid grid-cols-3 gap-4">
                                <Controller
                                    name="address"
                                    control={form.control}
                                    render={({ field, fieldState }) => (
                                        <Field data-invalid={fieldState.invalid} className="col-span-2">
                                            <FieldLabel htmlFor="address">Address</FieldLabel>
                                            <Input {...field} id="address" aria-invalid={fieldState.invalid} />
                                            {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
                                        </Field>
                                    )}
                                />
                                <Controller
                                    name="state"
                                    control={form.control}
                                    render={({ field, fieldState }) => (
                                        <Field data-invalid={fieldState.invalid}>
                                            <FieldLabel htmlFor="state">State</FieldLabel>
                                            <Input {...field} id="state" maxLength={2} className="uppercase" placeholder="CA" aria-invalid={fieldState.invalid} />
                                            {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
                                        </Field>
                                    )}
                                />
                            </div>

                            <Controller
                                name="companyName"
                                control={form.control}
                                render={({ field, fieldState }) => (
                                    <Field data-invalid={fieldState.invalid}>
                                        <FieldLabel htmlFor="companyName">Company Name</FieldLabel>
                                        <Input {...field} id="companyName" aria-invalid={fieldState.invalid} />
                                        {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
                                    </Field>
                                )}
                            />

                            <Controller
                                name="requestedAmount"
                                control={form.control}
                                render={({ field, fieldState }) => (
                                    <Field data-invalid={fieldState.invalid}>
                                        <FieldLabel htmlFor="requestedAmount">Requested Amount ($)</FieldLabel>
                                        <Input {...field} type="number" id="requestedAmount" aria-invalid={fieldState.invalid} />
                                        {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
                                    </Field>
                                )}
                            />

                            {serverError && (
                                <div className="text-sm font-medium text-destructive mt-2">{serverError}</div>
                            )}

                        </FieldGroup>
                    </form>
                </CardContent>
                <CardFooter>
                    <Button
                        type="submit"
                        form="loan-application-form"
                        className="w-full"
                        disabled={isSubmitting}
                    >
                        {isSubmitting ? "Processing..." : "Submit Application"}
                    </Button>
                </CardFooter>
            </Card>
        </main>
    )
}