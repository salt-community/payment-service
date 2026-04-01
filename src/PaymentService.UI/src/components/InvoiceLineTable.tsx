import { InvoiceLine } from "@/lib/types";

function formatCurrency(amount: number) {
  return `$${amount.toFixed(2)}`;
}

export function InvoiceLineTable({
  lines,
  total,
}: {
  lines: InvoiceLine[];
  total: number;
}) {
  return (
    <div className="win-panel-inset rounded-sm overflow-hidden">
      <table className="w-full text-sm">
        <thead>
          <tr className="bg-secondary text-secondary-foreground border-b border-border">
            <th className="text-left px-3 py-2 font-semibold">Name</th>
            <th className="text-left px-3 py-2 font-semibold hidden sm:table-cell">Service</th>
            <th className="text-right px-3 py-2 font-semibold">Unit Price</th>
            <th className="text-right px-3 py-2 font-semibold">Qty</th>
            <th className="text-right px-3 py-2 font-semibold">Total</th>
          </tr>
        </thead>
        <tbody>
          {lines.map((line) => (
            <tr key={line.id} className="border-b border-border last:border-0">
              <td className="px-3 py-2">{line.name}</td>
              <td className="px-3 py-2 text-muted-foreground hidden sm:table-cell">{line.serviceType}</td>
              <td className="px-3 py-2 text-right">{formatCurrency(line.unitPrice)}</td>
              <td className="px-3 py-2 text-right">{line.amount}</td>
              <td className="px-3 py-2 text-right font-medium">{formatCurrency(line.lineTotal)}</td>
            </tr>
          ))}
        </tbody>
        <tfoot>
          <tr className="bg-secondary">
            <td colSpan={3} className="px-3 py-2 font-bold text-right hidden sm:table-cell">Total</td>
            <td colSpan={2} className="px-3 py-2 font-bold text-right sm:hidden">Total</td>
            <td className="px-3 py-2 font-bold text-right hidden sm:table-cell">{formatCurrency(total)}</td>
            <td className="px-3 py-2 font-bold text-right sm:hidden">{formatCurrency(total)}</td>
          </tr>
        </tfoot>
      </table>
    </div>
  );
}
