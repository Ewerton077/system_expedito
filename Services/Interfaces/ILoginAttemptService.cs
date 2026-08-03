namespace SantoExpedito.Services.Interfaces;

public interface ILoginAttemptService
{
    bool IsBlocked(string ipAddress);
    void RecordFailedAttempt(string ipAddress);
    void Reset(string ipAddress);
}
