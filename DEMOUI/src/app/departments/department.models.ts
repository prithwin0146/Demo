export interface Department {
  departmentId: number;
  encryptedId: string;   // Encrypted ID for URL-safe operations
  departmentName: string;
  description: string | null;
  managerId: number | null;
  managerName: string | null;
  employeeCount: number;
}

export interface CreateDepartment {
  departmentName: string;
  description: string | null;
  managerId: number | null;
}

export interface UpdateDepartment {
  departmentName: string;
  description: string | null;
  managerId: number | null;
}
