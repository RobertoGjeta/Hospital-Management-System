import axiosClient from './axiosClient';

const RESOURCE = '/LabTest';

export const labTestsApi = {
  createOrder: (dto) => axiosClient.post(`${RESOURCE}/order`, dto).then((r) => r.data),
  getQueue: () => axiosClient.get(`${RESOURCE}/queue`).then((r) => r.data),
  uploadResult: (orderId, dto) => axiosClient.post(`${RESOURCE}/order/${orderId}/result`, dto).then((r) => r.data),
  releaseReport: (reportId) => axiosClient.put(`${RESOURCE}/report/${reportId}/release`).then((r) => r.data),
  getReleasedForPatient: (patientId) => axiosClient.get(`${RESOURCE}/patient/${patientId}/released`).then((r) => r.data),
  getReport: (reportId) => axiosClient.get(`${RESOURCE}/report/${reportId}`).then((r) => r.data),
};
