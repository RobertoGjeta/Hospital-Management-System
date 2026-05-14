import axiosClient from './axiosClient';

const RESOURCE = '/SampleCustody';

export const sampleCustodyApi = {
  create: (dto) => axiosClient.post(RESOURCE, dto).then((r) => r.data),
  getBySample: (sampleIdentifier) => axiosClient.get(`${RESOURCE}/${encodeURIComponent(sampleIdentifier)}`).then((r) => r.data),
};
