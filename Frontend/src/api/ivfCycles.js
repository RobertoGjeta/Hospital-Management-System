import axiosClient from './axiosClient';

const RESOURCE = '/IvfCycle';

export const ivfCyclesApi = {
  create: (dto) => axiosClient.post(RESOURCE, dto).then((r) => r.data),
  getByPatient: (patientId) => axiosClient.get(`${RESOURCE}/patient/${patientId}`).then((r) => r.data),
  advancePhase: (cycleId, doctorId, dto) =>
    axiosClient.put(`${RESOURCE}/${cycleId}/advance`, dto, { params: { doctorId } }).then((r) => r.data),
};
