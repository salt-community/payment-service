import { useParams, useNavigate } from "react-router-dom";
import { useInvoice, usePayInvoice } from "@/hooks/use-invoices";
import { Layout } from "@/components/Layout";
import { StatusBadge } from "@/components/StatusBadge";
import { InvoiceLineTable } from "@/components/InvoiceLineTable";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { toast } from "sonner";
import { ArrowLeft } from "lucide-react";

function formatDate(iso: string) {
  return new Date(iso).toLocaleDateString("en-US", {
    year: "numeric",
    month: "short",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

export default function InvoiceDetailsPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { data: invoice, isLoading, error } = useInvoice(id || "");
  const payMutation = usePayInvoice();

  const handlePay = () => {
    if (!id) return;
    payMutation.mutate(id, {
      onSuccess: () => toast.success("Payment successful!"),
      onError: () => toast.error("Payment failed. Please try again."),
    });
  };

  return (
    <Layout>
      <button
        onClick={() => navigate("/")}
        className="flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground mb-4 transition-colors"
      >
        <ArrowLeft className="w-4 h-4" />
        Back to Invoices
      </button>

      {error && (
        <div className="win-panel-inset rounded-sm p-4 text-destructive">
          Invoice not found.
        </div>
      )}

      {isLoading && (
        <div className="space-y-3">
          <Skeleton className="h-6 w-48 rounded-sm" />
          <Skeleton className="h-40 w-full rounded-sm" />
          <Skeleton className="h-32 w-full rounded-sm" />
        </div>
      )}

      {invoice && (
        <div className="space-y-5">
          <div className="flex items-center justify-between">
            <h1 className="text-lg font-bold">{invoice.id}</h1>
            <StatusBadge status={invoice.status} />
          </div>

          <div className="win-panel-inset rounded-sm p-4">
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-3 text-sm">
              <Field label="Booking ID" value={invoice.bookingId} />
              <Field label="Customer" value={invoice.customerName} />
              <Field label="Email" value={invoice.customerEmail} />
              <Field label="Created" value={formatDate(invoice.createdAt)} />
              <Field label="Last Updated" value={formatDate(invoice.updatedAt)} />
              <Field label="Status" value={invoice.status} />
            </div>
          </div>

          <div>
            <h2 className="text-sm font-bold mb-2">Line Items</h2>
            <InvoiceLineTable lines={invoice.lines} total={invoice.totalAmount} />
          </div>

          <div className="flex justify-end pt-2">
            <Button
              onClick={handlePay}
              disabled={invoice.status === "Paid" || payMutation.isPending}
              className="min-w-[120px]"
            >
              {payMutation.isPending
                ? "Processing…"
                : invoice.status === "Paid"
                ? "Paid"
                : "Pay Now"}
            </Button>
          </div>
        </div>
      )}
    </Layout>
  );
}

function Field({ label, value }: { label: string; value: string }) {
  return (
    <div>
      <span className="text-muted-foreground">{label}:</span>{" "}
      <span className="font-medium">{value}</span>
    </div>
  );
}
