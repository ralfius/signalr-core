import { MessageActionType, MessageType } from '../enums';

export class InstantMessage {
    actionType: MessageActionType;
    messageType: MessageType;
    larId: number;
    userId: string;
    accountId: string;
    larName: string;
    dateTime: Date;
    itemId: number;
    payload: JSON;
}
