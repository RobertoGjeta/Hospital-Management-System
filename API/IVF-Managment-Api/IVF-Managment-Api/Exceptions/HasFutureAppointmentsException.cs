namespace IVF_Managment_Api.Exceptions;

public class HasFutureAppointmentsException : Exception
{
    public HasFutureAppointmentsException()
        : base("Doctor has future appointments and cannot be deactivated.") { }
}