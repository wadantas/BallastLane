export interface Vehicle {
  id: string;
  plateNumber: string;
  document: string;
  brand: string;
  model: string;
  year: number;
  price: number;
  isSold: boolean;
  createdAt: string;
  updatedAt: string | null;
}
