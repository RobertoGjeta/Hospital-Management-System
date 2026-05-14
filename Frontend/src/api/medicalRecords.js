import axiosClient from './axiosClient';

const RESOURCE = '/MedicalRecord';

export const medicalRecordsApi = {
  create: (dto) => axiosClient.post(RESOURCE, dto).then((r) => r.data),
  getByPatient: (patientId) => axiosClient.get(`${RESOURCE}/patient/${patientId}`).then((r) => r.data),
};
