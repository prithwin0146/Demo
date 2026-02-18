export interface Project {
  projectId: number;
  encryptedId: string;   // Encrypted ID for URL-safe operations
  projectName: string;
  description: string;
  startDate: string;
  endDate: string | null;
  status: string;
  assignedEmployees?: number;
  employeeNames?: string | null;
}

export interface CreateProject {
  projectName: string;
  description: string;
  startDate: string;
  endDate: string | null;
  status: string;
}

export interface UpdateProject {
  projectName: string;
  description: string;
  startDate: string;
  endDate: string | null;
  status: string;
}