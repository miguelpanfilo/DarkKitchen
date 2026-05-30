export interface TopProduct {
    code: string;
    name: string;
    quantitySold: number;
    images: string[];
  }

  export interface SalesCustomer {
    customer: string;
    totalSales: number;
  }

  export interface SalesPeriod {
    year: number;
    month: number;
    customers: SalesCustomer[];
    monthTotal: number;
  }

  export interface SalesReport {
    periods: SalesPeriod[];
    grandTotal: number;
  }