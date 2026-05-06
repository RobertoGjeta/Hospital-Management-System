export const mockPatient = {
  name: 'Sarah Mitchell',
  patientSystemId: 'P-20240312',
  age: 34,
  cycleDay: 8,
  protocol: 'Antagonist',
  viralStatus: 'Negative',
};

export const mockStimulationData = [
  { day: 1,  follicle: 5.2,  estradiol: 120  },
  { day: 2,  follicle: 5.8,  estradiol: 210  },
  { day: 3,  follicle: 6.5,  estradiol: 340  },
  { day: 4,  follicle: 7.8,  estradiol: 520  },
  { day: 5,  follicle: 9.4,  estradiol: 720  },
  { day: 6,  follicle: 11.0, estradiol: 980  },
  { day: 7,  follicle: 12.5, estradiol: 1200 },
  { day: 8,  follicle: 14.2, estradiol: 1520 },
  { day: 9,  follicle: 15.8, estradiol: 1820 },
  { day: 10, follicle: 17.1, estradiol: 2100 },
  { day: 11, follicle: 18.5, estradiol: 2450 },
  { day: 12, follicle: 20.2, estradiol: 2800 },
];

export const mockTanks = [
  { id: 'Tank-01', level: 92, status: 'ok' },
  { id: 'Tank-02', level: 78, status: 'low' },
];

export const mockEmbryologyTimeline = [
  { day: 'Day 0', label: 'Retrieval',      grade: null,      status: 'complete', cells: null },
  { day: 'Day 1', label: 'Fertilisation',  grade: null,      status: 'complete', cells: '2PN' },
  { day: 'Day 2', label: '',               grade: 'Grade A', status: 'complete', cells: '4' },
  { day: 'Day 3', label: '',               grade: 'Grade A', status: 'complete', cells: '8' },
  { day: 'Day 4', label: 'Morula',         grade: 'Started', status: 'active',   cells: 'M' },
  { day: 'Day 5', label: 'Blastocyst',     grade: null,      status: 'pending',  cells: null },
  { day: 'Day 6', label: 'Transfer/Cryo',  grade: null,      status: 'pending',  cells: null },
];

export const mockViableEmbryos = [
  { id: 'E001', day: 5, grade: '5AA', score: 9.2, recommendation: 'Excellent for fresh transfer' },
  { id: 'E002', day: 5, grade: '4AA', score: 8.8, recommendation: 'Good for transfer or freeze' },
  { id: 'E003', day: 6, grade: '5AB', score: 8.5, recommendation: 'Freeze - excellent quality' },
];

export const mockPrognosis = {
  ohss: {
    level: 'High',
    message: 'Risk of OHSS: High — Consider Trigger Modification',
    detail: 'Estradiol >2500 pg/mL, >15 follicles >12mm. Recommend GnRH agonist trigger.',
  },
  trigger: {
    day: 10,
    message: 'Recommended trigger: Day 10 (36 hours prior to retrieval)',
    detail: 'Based on follicle cohort size and estradiol trajectory.',
  },
  transfer: {
    probability: 87,
    message: '87% probability of high-quality blastocysts',
    detail: 'AI model confidence based on patient age, AMH, and current cycle parameters.',
  },
};
