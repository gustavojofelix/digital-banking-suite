export interface EmployeesQuery {
  pageNumber: number;
  pageSize: number;
  search: string;
  includeInactive: boolean;
}
