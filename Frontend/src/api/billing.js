import axiosClient from './axiosClient';

const RESOURCE = '/Billing';

export const billingApi = {
  createBill: (dto) => axiosClient.post(RESOURCE, dto).then((r) => r.data),
  getById: (id) => axiosClient.get(`${RESOURCE}/${id}`).then((r) => r.data),
  getByPatient: (patientId) => axiosClient.get(`${RESOURCE}/patient/${patientId}`).then((r) => r.data),
  addLineItem: (dto) => axiosClient.post(`${RESOURCE}/line-items`, dto).then((r) => r.data),
  recordPayment: (dto) => axiosClient.post(`${RESOURCE}/payments`, dto).then((r) => r.data),
};
