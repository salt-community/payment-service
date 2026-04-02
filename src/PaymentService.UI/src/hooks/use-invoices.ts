import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { Invoice } from "@/lib/types";
import { mockInvoices } from "@/lib/mock-data";

const API_BASE_URL = "http://localhost:5093/api/payment";
// Simulated API calls

const fetchInvoices = async (): Promise<Invoice[]> => {
  const res = await fetch(`${API_BASE_URL}`);
  if (!res.ok) throw new Error("Failed to fetch invoices");
  return res.json();
};

const fetchInvoice = async (id: string): Promise<Invoice> => {
  const res = await fetch(`${API_BASE_URL}/${id}`);
  if (!res.ok) throw new Error("Invoice not found");
  return res.json();
};

const payInvoice = async (id: string): Promise<Invoice> => {
  const res = await fetch(`${API_BASE_URL}/${id}/pay`, {
    method: "POST",
  });

  if (!res.ok) throw new Error("Payment failed");
  console.log("Payment response is....", res);
  return fetchInvoice(id);
};

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
