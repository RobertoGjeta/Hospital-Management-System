namespace IVFClinic.Models.Enums
{
    public enum UserRole
    {
        Admin = 0,
        Doctor = 1,
        Patient = 2,
        LabTechnician = 3
    }

    public enum Gender
    {
        Male = 0,
        Female = 1,
        Other = 2
    }

    public enum AuditAction
    {
        Create = 0,
        Update = 1,
        Delete = 2,
        Deactivate = 3,
        Login = 4,
        LoginFailed = 5,
        PasswordChange = 6,
        GenerateBill = 7,
        RecordPayment = 8,
        AssignRoom = 9,
        GenerateReport = 10
    }

    public enum AppointmentStatus
    {
        Scheduled = 0,
        Confirmed = 1,
        CheckedIn = 2,
        InProgress = 3,
        Completed = 4,
        Cancelled = 5,
        NoShow = 6
    }

    public enum RoomType
    {
        Consultation = 0,
        Procedure = 1,
        Lab = 2,
        Recovery = 3,
        OperatingRoom = 4
    }

    public enum RoomStatus
    {
        Available = 0,
        Occupied = 1,
        Maintenance = 2,
        Reserved = 3
    }

    public enum BillStatus
    {
        Pending = 0,
        PartiallyPaid = 1,
        Paid = 2,
        Cancelled = 3,
        Refunded = 4
    }

    public enum PaymentMethod
    {
        Cash = 0,
        CreditCard = 1,
        DebitCard = 2,
        BankTransfer = 3,
        Insurance = 4,
        Other = 5
    }

    public enum ServiceCategory
    {
        Consultation = 0,
        LabTest = 1,
        Procedure = 2,
        Medication = 3,
        Imaging = 4,
        Other = 5
    }
}
