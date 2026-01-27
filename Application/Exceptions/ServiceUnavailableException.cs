namespace Application.Exceptions;

// ✅ Bu exception - "xizmat vaqtincha mavjud emas" degani.
// Masalan: Database yoki Redis umuman ishlamayapti.
public class ServiceUnavailableException : Exception
{
    public ServiceUnavailableException(string message, Exception? inner = null)
        : base(message, inner) { }
}