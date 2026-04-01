import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { Invoice } from "@/lib/types";
import { mockInvoices } from "@/lib/mock-data";

// Simulated API calls
const fetchInvoices = (): Promise<Invoice[]> =>
  new Promise((resolve) => setTimeout(() => resolve([...mockInvoices]), 600));

const fetchInvoice = (id: string): Promise<Invoice> =>
  new Promise((resolve, reject) => {
    setTimeout(() => {
      const invoice = mockInvoices.find((inv) => inv.id === id);
      if (invoice) resolve({ ...invoice });
      else reject(new Error("Invoice not found"));
    }, 400);
  });

const payInvoice = (id: string): Promise<Invoice> =>
  new Promise((resolve, reject) => {
    setTimeout(() => {
      const invoice = mockInvoices.find((inv) => inv.id === id);
      if (!invoice) return reject(new Error("Invoice not found"));
      invoice.status = "Paid";
      invoice.updatedAt = new Date().toISOString();
      resolve({ ...invoice });
    }, 1200);
  });

export function useInvoices() {
  return useQuery({
    queryKey: ["invoices"],
    queryFn: fetchInvoices,
  });
}

export function useInvoice(id: string) {
  return useQuery({
    queryKey: ["invoice", id],
    queryFn: () => fetchInvoice(id),
    enabled: !!id,
  });
}

export function usePayInvoice() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: payInvoice,
    onSuccess: (updatedInvoice) => {
      queryClient.setQueryData(["invoice", updatedInvoice.id], updatedInvoice);
      queryClient.invalidateQueries({ queryKey: ["invoices"] });
    },
  });
}
