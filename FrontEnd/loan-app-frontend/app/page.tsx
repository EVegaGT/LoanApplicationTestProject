import { Button } from "@/components/ui/button";
import Link from "next/link";

export default function Home() {
  return (
    <div className="flex flex-col flex-1 items-center justify-center bg-zinc-50 font-sans dark:bg-black">
      <main className="flex flex-1 w-full max-w-3xl flex-col items-center justify-between py-32 px-16 bg-white dark:bg-black sm:items-start">
        <h1 className="text-5xl font-bold text-black dark:text-white sm:text-6xl">
          Welcome to{" "}
          <p className="text-blue-600 dark:text-blue-400">
            Loan Application
          </p>
          <Link href="/loan">
            <Button variant="default" size="lg" className="text-3xl px-6 py-5 mt-8 font-bold text-white">
              Request A New Loan
            </Button>
          </Link>

        </h1>F
      </main>
    </div>
  );
}
