export interface AuditLog {
    dateTime: string;
    entityName: string;
    entityId: number;
    changeDescription: string;
    responsibleUser: string;
  }