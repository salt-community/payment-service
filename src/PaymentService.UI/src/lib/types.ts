export type PaymentStatus = "Paid" | "Pending" | "Failed";

export interface InvoiceLine {
  id: string;
  name: string;
  serviceType: string;
  unitPrice: number;
  amount: number;
  lineTotal: number;
}

export interface Invoice {
  id: string;
  bookingId: string;
  customerName: string;
  customerEmail: string;
  totalAmount: number;
  status: PaymentStatus;
  createdAt: string;
  updatedAt: string;
  lines: InvoiceLine[];
}
