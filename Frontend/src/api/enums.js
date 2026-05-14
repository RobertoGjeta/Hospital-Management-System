function labels(map) {
  return Object.fromEntries(Object.entries(map).map(([k, v]) => [Number(k), v]));
}

export const UserRole = { Administrator: 0, Doctor: 1, LabTechnician: 2, Patient: 3 };
export const UserRoleLabel = labels({ 0: 'Administrator', 1: 'Doctor', 2: 'Lab Technician', 3: 'Patient' });

export const Gender = { Male: 0, Female: 1, Other: 2 };
export const GenderLabel = labels({ 0: 'Male', 1: 'Female', 2: 'Other' });

export const BillingType = { Insurance: 0, SelfPay: 1 };
export const BillingTypeLabel = labels({ 0: 'Insurance', 1: 'Self-Pay' });

export const AppointmentStatus = { Confirmed: 0, Completed: 1, Cancelled: 2, Rescheduled: 3, Scheduled: 4, NoShow: 5 };
export const AppointmentStatusLabel = labels({
  0: 'Confirmed', 1: 'Completed', 2: 'Cancelled', 3: 'Rescheduled', 4: 'Scheduled', 5: 'No Show',
});

export const TestPriority = { Routine: 0, Urgent: 1, Stat: 2 };
export const TestPriorityLabel = labels({ 0: 'Routine', 1: 'Urgent', 2: 'Stat' });

export const TestStatus = { Pending: 0, InProgress: 1, Completed: 2, Cancelled: 3 };
export const TestStatusLabel = labels({ 0: 'Pending', 1: 'In Progress', 2: 'Completed', 3: 'Cancelled' });

export const RecordEntryType = { ClinicalNote: 0, Diagnosis: 1, Prescription: 2, TreatmentPlan: 3, TreatmentPlanUpdate: 4 };
export const RecordEntryTypeLabel = labels({
  0: 'Clinical Note', 1: 'Diagnosis', 2: 'Prescription', 3: 'Treatment Plan', 4: 'Treatment Plan Update',
});

export const CyclePhase = {
  Stimulation: 0, EggRetrieval: 1, Fertilization: 2, EmbryoCulture: 3, Transfer: 4, LutealPhase: 5, Completed: 6, Cancelled: 7,
};
export const CyclePhaseLabel = labels({
  0: 'Stimulation', 1: 'Egg Retrieval', 2: 'Fertilization', 3: 'Embryo Culture',
  4: 'Transfer', 5: 'Luteal Phase', 6: 'Completed', 7: 'Cancelled',
});

export const EmbryoInstructionType = { Transfer: 0, FET: 1, Cryopreserve: 2, Discard: 3, Await: 4 };
export const EmbryoInstructionTypeLabel = labels({
  0: 'Transfer', 1: 'FET', 2: 'Cryopreserve', 3: 'Discard', 4: 'Await',
});

export const EmbryoStatus = { Developing: 0, Frozen: 1, Transferred: 2, Discarded: 3, Arrested: 4 };
export const EmbryoStatusLabel = labels({
  0: 'Developing', 1: 'Frozen', 2: 'Transferred', 3: 'Discarded', 4: 'Arrested',
});

export const BillStatus = { Pending: 0, PartiallyPaid: 1, Paid: 2 };
export const BillStatusLabel = labels({ 0: 'Pending', 1: 'Partially Paid', 2: 'Paid' });

export const PaymentMethod = { Cash: 0, Card: 1, BankTransfer: 2, Insurance: 3 };
export const PaymentMethodLabel = labels({ 0: 'Cash', 1: 'Card', 2: 'Bank Transfer', 3: 'Insurance' });

export const CustodyEventType = { Received: 0, Processed: 1, Stored: 2, Transferred: 3, Discarded: 4 };
export const CustodyEventTypeLabel = labels({
  0: 'Received', 1: 'Processed', 2: 'Stored', 3: 'Transferred', 4: 'Discarded',
});

export const NotificationType = {
  General: 0, UrgentLabResult: 1, AppointmentScheduled: 2, AppointmentCancelled: 3, AppointmentRescheduled: 4,
};
export const NotificationTypeLabel = labels({
  0: 'General', 1: 'Urgent Lab Result', 2: 'Appointment Scheduled', 3: 'Appointment Cancelled', 4: 'Appointment Rescheduled',
});

export const DonationSampleType = { Sperm: 0, Egg: 1, Embryo: 2 };
export const DonationSampleTypeLabel = labels({ 0: 'Sperm', 1: 'Egg', 2: 'Embryo' });

export const ScreeningStatus = { Pending: 0, Cleared: 1, Rejected: 2 };
export const ScreeningStatusLabel = labels({ 0: 'Pending', 1: 'Cleared', 2: 'Rejected' });
