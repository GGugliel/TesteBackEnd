namespace Domain.Interfaces;

public interface IEmailService
{
    void SendEmail(string to, string message);   
}