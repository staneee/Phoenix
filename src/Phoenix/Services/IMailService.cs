using System.Threading.Tasks;

namespace Phoenix.Services
{
  public interface IMailService
  {
    Task SendMail(string template, string name, string email, string subject, string msg);
  }
}