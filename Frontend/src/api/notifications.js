import axiosClient from './axiosClient';

const RESOURCE = '/Notification';

export const notificationsApi = {
  getUnread: (userId) => axiosClient.get(`${RESOURCE}/unread/${userId}`).then((r) => r.data),
  markRead: (notificationId) => axiosClient.put(`${RESOURCE}/${notificationId}/read`).then((r) => r.data),
};
