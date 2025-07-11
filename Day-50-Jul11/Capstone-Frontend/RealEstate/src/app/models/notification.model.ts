export interface NotificationModel {
  id: string;
  title: string;
  message: string;
  type: 'property' | 'inquiry' | 'system';
  isRead: boolean;
  timestamp: Date;
  propertyId?: string;
  location?: string;
  agentName?: string;
}

export interface PropertyNotification {
  propertyId: string;
  title: string;
  location: string;
  price: number;
  agentName: string;
  agentId: string;
  timestamp: Date;
}