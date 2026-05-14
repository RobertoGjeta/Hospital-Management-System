namespace IVF_Managment_Api.Exceptions;

public class DuplicatePatientException : Exception
{
    public IReadOnlyList<Guid> ExistingPatientIds { get; }

    public DuplicatePatientException(IEnumerable<Guid> existingIds)
        : base("Duplicate patient detected.")
    {
        ExistingPatientIds = existingIds.ToList().AsReadOnly();
    }
}