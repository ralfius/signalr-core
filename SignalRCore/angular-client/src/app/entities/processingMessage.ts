import { NotificationSource, Status } from '../enums';

export class ProcessingMessage {
    dateTime: Date;
    processId: string;
    processType: NotificationSource;
    status: Status;
    percentage: number;
    larId: number;
    larName: string;
    userId: string;
    accountId: string;
    payload: JSON;
}
