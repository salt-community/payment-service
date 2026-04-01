import { useNavigate } from "react-router-dom";
import { useInvoices } from "@/hooks/use-invoices";
import { Layout } from "@/components/Layout";
import { StatusBadge } from "@/components/StatusBadge";
import { Skeleton } from "@/components/ui/skeleton";

function formatDate(iso: string) {
  return new Date(iso).toLocaleDateString("en-US", {
    year: "numeric",
    month: "short",
    day: "numeric",
  });
}

export default function InvoiceListPage() {
  const { data: invoices, isLoading, error } = useInvoices();
  const navigate = useNavigate();

  return (
    <Layout>
      <h1 className="text-lg font-bold mb-4">Invoices</h1>

      {error && (
        <div className="win-panel-inset rounded-sm p-4 text-destructive">
          Failed to load invoices. Please try again.
        </div>
      )}

      {isLoading && (
        <div className="space-y-2">
          {[1, 2, 3].map((i) => (
            <Skeleton key={i} className="h-14 w-full rounded-sm" />
          ))}
        </div>
      )}

      {invoices && (
        <div className="win-panel-inset rounded-sm overflow-hidden">
          <table className="w-full text-sm">
            <thead>
              <tr className="bg-secondary text-secondary-foreground border-b border-border">
                <th className="text-left px-3 py-2 font-semibold">Invoice</th>
                <th className="text-left px-3 py-2 font-semibold hidden sm:table-cell">Customer</th>
                <th className="text-right px-3 py-2 font-semibold">Amount</th>
                <th className="text-center px-3 py-2 font-semibold">Status</th>
                <th className="text-left px-3 py-2 font-semibold hidden md:table-cell">Date</th>
              </tr>
            </thead>
            <tbody>
              {invoices.map((inv) => (
                <tr
                  key={inv.id}
                  onClick={() => navigate(`/invoice/${inv.id}`)}
                  className="border-b border-border last:border-0 cursor-pointer hover:bg-accent transition-colors"
                >
                  <td className="px-3 py-2.5 font-medium">{inv.id}</td>
                  <td className="px-3 py-2.5 hidden sm:table-cell">{inv.customerName}</td>
                  <td className="px-3 py-2.5 text-right">${inv.totalAmount.toFixed(2)}</td>
                  <td className="px-3 py-2.5 text-center">
                    <StatusBadge status={inv.status} />
                  </td>
                  <td className="px-3 py-2.5 text-muted-foreground hidden md:table-cell">
                    {formatDate(inv.createdAt)}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </Layout>
  );
}
