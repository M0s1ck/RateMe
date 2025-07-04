namespace RateMeApiServer.Common;

public class DbInteractionResult<T>
{
    public T? Value { get; }
    public DbInteractionStatus Status { get; }

    public DbInteractionResult(T? value, DbInteractionStatus status)
    {
        Value = value;
        Status = status;
    }
}