import axiosClient from './axiosClient';

const RESOURCE = '/DonationBank';

export const donationBankApi = {
  create: (dto) => axiosClient.post(RESOURCE, dto).then((r) => r.data),
  getByDonor: (donorId) => axiosClient.get(`${RESOURCE}/donor/${donorId}`).then((r) => r.data),
  getAssignable: () => axiosClient.get(`${RESOURCE}/assignable`).then((r) => r.data),
  updateScreening: (sampleId, dto) => axiosClient.put(`${RESOURCE}/${sampleId}/screening`, dto).then((r) => r.data),
  updateQuantity: (sampleId, quantity) => axiosClient.put(`${RESOURCE}/${sampleId}/quantity`, quantity).then((r) => r.data),
};
