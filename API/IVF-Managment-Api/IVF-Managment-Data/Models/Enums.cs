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
        Rescheduled
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
        UnderReview,
        Available
        
    }

    public enum RecordEntryType
    {
        ClinicalNote,
        Diagnosis,
        Prescription,
        TreatmentPlan
    }

    public enum CyclePhase
    {
        Stimulation,
        EggRetrieval,
        Fertilization,
        EmbryoCulture,
        Transfer,
        LutealPhase
        
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
}