
namespace Assets.CommunicationSystem
{

	public delegate void SetUserRoleInfoRespond(int errorCode);

	public interface IUserInfo
	{
		string nickName { get; set; }
		SexType gender { get; set; }
		bool isNewUser { get; }
		int roleId { get; }
		void requestSetUserRoleInfo();
		event SetUserRoleInfoRespond onSetUserRoleInfo;
	}

	public delegate void onGatewayServerResponded(IGatewayServerAuthenticationRespondData data);
	public delegate void onGatewayServerConnected(int errorCode);

	public interface IGatewayServerAuthenticate
	{
		void connect(bool isAsync);
		void disconnect();
		void postAuthRequest(IGatewayServerAuthenticationRequestData dataItf);
		event onGatewayServerResponded onResponded;
		event onGatewayServerConnected onConnected;
	}



	public delegate void onGameServerConnected(int errorCode);
	public interface IGameServerConnector
	{
		IGameServerConnectorInfo connectInfo { get; set; }
		void connect(bool isAsync);
		void reconnect(bool isAsync);
		void disconnect();
		event onGameServerConnected onConnected;
		event onGameServerConnected onReconnected;
	}

	public interface IGameServerConnectorInfo
	{
		string ipAddr { get; }
		int port { get; }
		int playerId { get; }
		string session { get; }
	}

	public interface IGatewayServerAuthenticationRequestData
	{
		string uniqueClientCode { get; }
		int currentPlatformID { get; }
	}

	public interface IGatewayServerAuthenticationRespondData
	{
		string targetServerIpAddr { get; }
		int targetServerPort { get; }
		string targetServerAccessToken { get; }
		int errCode { get; }
		int playerId { get; }
	}

	public interface IHttpRequest
	{
		string url { get; }
		string port { get; }
		string method { get; }

		void request();
	}


	public delegate void onGameLoginResult(IUserInfo info);

	public interface ILoginRequest
	{
		void login(int playerId);
		event onGameLoginResult onLoginRespond;
	}
}
