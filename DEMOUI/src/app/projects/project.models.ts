export interface Project {
  projectId: number;
  projectName: string;
  description: string;
  startDate: string;
  endDate: string | null;
  status: string;
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
