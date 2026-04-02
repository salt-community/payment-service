import { PaymentStatus } from "@/lib/types";
import { cn } from "@/lib/utils";

const statusStyles: Record<PaymentStatus, string> = {
  Paid: "bg-status-paid text-status-paid-foreground",
  Invoiced: "bg-status-pending text-status-pending-foreground",
  Failed: "bg-status-failed text-status-failed-foreground",
};

export function StatusBadge({ status }: { status: PaymentStatus }) {
  return (
    <span
      className={cn(
        "inline-block px-2.5 py-0.5 text-xs font-semibold rounded-sm",
        statusStyles[status],
      )}
    >
      {status}
    </span>
  );
}
