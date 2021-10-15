using System.Threading.Tasks;

namespace HackathonE1.Website.Client.Auth
{
	public interface ILoginManager
	{
		Task<bool> Login( string username, string password );
		Task Logout();
	}
}