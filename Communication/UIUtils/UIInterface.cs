
using Framework.System.Communication;

namespace Assets.LoginSystem.UIUtils
{

	public delegate void LoginUIProcessResult(IGatewayServerAuthenticationRequestData iData);
	public delegate void LoginUIAnonymousUserRequestResult(string anonymousCredits);
	public interface ILoginUI
	{
		string password { get; set; }
		string username { get; set; }
		void login(bool isAnonymous, bool isAsyncLogin);
		event LoginUIProcessResult onLoginResult;
		void storeUserCredits();
		void restoreUserCredits();
		void getAnonymousIdFromLoginServer(bool isAsync);
		event LoginUIAnonymousUserRequestResult onAnonymousCredits;

	}

	public interface ISetUserInfoUI
	{
		string userName { set; }
		//PlayerSex userSex { set; }
	}
}
