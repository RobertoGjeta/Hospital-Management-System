using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IvfClinic.Models
{
    public enum UserRole 
    { 
        Administrator, 
        Doctor, 
        LabTechnician, 
        Patient 
    }

    public enum Gender 
    { 
        Male, 
        Female, 
        Other 
    }

    public enum BillingType 
    { 
        Insurance, 
        SelfPay 
    }

    public enum AppointmentStatus
    {
        Confirmed,
        Completed,
        Cancelled,
        Rescheduled,
        Scheduled,
        NoShow
    }

    public enum TestPriority
    {
        Routine,
        Urgent,
        Stat
    }

    public enum TestStatus
    {
        Pending,
        InProgress,
        Completed,
        Cancelled
    }

    public enum RecordEntryType
    {
        ClinicalNote,
        Diagnosis,
        Prescription,
        TreatmentPlan,
        TreatmentPlanUpdate
    }

    public enum CyclePhase
    {
        Stimulation,
        EggRetrieval,
        Fertilization,
        EmbryoCulture,
        Transfer,
        LutealPhase,
        Completed,
        Cancelled
    }

    public enum EmbryoInstructionType
    {
        Transfer,
        FET,
        Cryopreserve,
        Discard,
        Await
    }

    public enum EmbryoStatus
    {
        Developing,
        Frozen,
        Transferred,
        Discarded,
        Arrested
    }

    public enum BillStatus
    {
        Pending,
        PartiallyPaid,
        Paid
    }

    public enum PaymentMethod
    {
        Cash,
        Card, 
        BankTransfer,
        Insurance
        
        
    }
    
   
    public enum CustodyEventType
    {
        Received,
        Processed,
        Stored,
        Transferred,
        Discarded
    }

    public enum NotificationType
    {
        General,
        UrgentLabResult,
        AppointmentScheduled,
        AppointmentCancelled,
        AppointmentRescheduled
    }

    public enum NotificationChannel
    {
        InApp,
        Email,
        SMS
    }

    public enum DonationSampleType
    {
        Sperm,
        Egg,
        Embryo
    }

    public enum ScreeningStatus
    {
        Pending,
        Cleared,
        Rejected
    }
}