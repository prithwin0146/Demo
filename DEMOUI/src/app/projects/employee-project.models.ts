export interface EmployeeProjectDto {
  employeeProjectId: number;
  encryptedEmployeeId: string;  // Encrypted ID for URL-safe operations
  encryptedProjectId: string;   // Encrypted ID for URL-safe operations
  employeeId: number;
  employeeName: string;
  projectId: number;
  projectName: string;
  assignedDate: string;
  role: string | null;
}

export interface AssignEmployeeDto {
  employeeId: number;
  projectId: number;
  role: string | null;
}
