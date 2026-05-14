import axiosClient from './axiosClient';

const RESOURCE = '/Auth';

export const authApi = {
  login: (dto) => axiosClient.post(`${RESOURCE}/login`, dto).then((r) => r.data),
  registerPatient: (dto) => axiosClient.post(`${RESOURCE}/register/patient`, dto).then((r) => r.data),
  adminRegister: (dto) => axiosClient.post(`${RESOURCE}/register`, dto).then((r) => r.data),
};
