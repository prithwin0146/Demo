export interface EmployeeProjectDto {
  employeeProjectId: number;
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
