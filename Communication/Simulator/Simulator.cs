

using System;
using Proto;
using UnityEngine;

namespace Assets.CommunicationSystem.Simulator
{

	public class GatewayServerAuthenticateSimulator : AGatewayServerAuthenticate
	{
		static readonly string IPADDR = "10.0.128.169";
		static readonly int PORT = 8620;

		public GatewayServerAuthenticateSimulator()
		{
			
		}

		protected override void disconnect()
		{
			NetWorkInstance.Instance.LeaveNetwork();
		}


		private void _connect_fun()
		{
			string ipAddr = IPADDR;
			int port = PORT;

			ipAddr = GameInstance.Instance.ipAddress;
			port = GameInstance.Instance.port;

			if (string.IsNullOrEmpty(ipAddr))
			{
				ipAddr = GameInstance.Instance.exIpAddress;
				port = GameInstance.Instance.exPort;
			}

			NetWorkInstance.Instance.CreateConnector(ConnectorMode.NM_C_INTRANET, ipAddr, port);
			triggerConnectionEvent(0);
		}

		protected override void asyncConnect()
		{
			_connect_fun();
		}
		protected override void syncConnect()
		{
			_connect_fun();
		}


		protected override void onConnectionResult(int errorCode)
		{

		}

		protected override void send(string uniCode, int platformId)
		{
			NetWorkInstance.Instance.GatewayAuth(uniCode, platformId);
			NetworkEventMgr.Instance.RemoveListener(Proto.CSProtoID.RSP_PLAYER_AUTH, msgCallback);
			NetworkEventMgr.Instance.AddListener(Proto.CSProtoID.RSP_PLAYER_AUTH, msgCallback);
		}

		private void msgCallback(ProtoBuf.IExtensible msgArgs)
		{
			NetworkEventMgr.Instance.RemoveListener(Proto.CSProtoID.RSP_PLAYER_AUTH, msgCallback);
			Proto.RspPlayerAuth data = msgArgs as Proto.RspPlayerAuth;
			if (data != null)
			{
				GatewayServerAuthenticationRespondData authData = new GatewayServerAuthenticationRespondData();
				authData.ip = data.ip;
				authData.port = data.port;
				authData.accessToken = data.token;
				authData.errCode = data.resultID;
				authData.playerId = data.pid;
				pumpRespondData(authData);
			}
		}

		protected override IGatewayServerAuthenticationRequestData getDefaultRequestData()
		{
			return new GatewayAuthRequestDataSimulator();
		}
	}

	public class GatewayAuthRequestDataSimulator : IGatewayServerAuthenticationRequestData
	{

		string IGatewayServerAuthenticationRequestData.uniqueClientCode
		{
			get
			{
				return _clientKey;
			}
		}

		int IGatewayServerAuthenticationRequestData.currentPlatformID
		{
			get
			{
				return 1;
			}
		}

		string _clientKey = null;
		public GatewayAuthRequestDataSimulator()
		{
			_clientKey = "";
		}

		public GatewayAuthRequestDataSimulator(string clientKey)
		{
			_clientKey = string.IsNullOrEmpty(clientKey) ? "" : clientKey;
		}

		public GatewayAuthRequestDataSimulator(string username, string password)
		{
			_clientKey = username + "|_|->" + password;
		}
	}


	public class GameServerConnectorSimulator : AGameServerConnector
	{
		public GameServerConnectorSimulator()
		{

		}

		public override void disconnect()
		{
			NetWorkInstance.Instance.LeaveNetwork();
		}

		protected override void asyncConnect(IGameServerConnectorInfo connectInfo)
		{
			NetWorkInstance.Instance.CreateConnector(ConnectorMode.NM_C_INTRANET, connectInfo.ipAddr, connectInfo.port);
			triggerConnectionEvent(0);
		}
		protected override void syncConnect(IGameServerConnectorInfo connectInfo)
		{
			NetWorkInstance.Instance.CreateConnector(ConnectorMode.NM_C_INTRANET, connectInfo.ipAddr, connectInfo.port);
			triggerConnectionEvent(0);
		}


		protected override void onConnectionResult(int errorCode)
		{

		}

		protected override void onReconnectionResult(int errorCode)
		{
			throw new NotImplementedException();
		}

		protected override void syncReonnect(IGameServerConnectorInfo connectInfo)
		{
			throw new NotImplementedException();
		}

		protected override void asyncReconnect(IGameServerConnectorInfo connectInfo)
		{
			throw new NotImplementedException();
		}
	}

	public class GameServerConnectorInfoSimulator : IGameServerConnectorInfo
	{
		string IGameServerConnectorInfo.ipAddr
		{
			get
			{
				return this.ip_;
			}
		}

		int IGameServerConnectorInfo.port
		{
			get
			{
				return this.port_;
			}
		}

		int IGameServerConnectorInfo.playerId
		{
			get
			{
				return this.playerId_;
			}
		}

		string IGameServerConnectorInfo.session
		{
			get
			{
				return this.session_;
			}
		}

		string ip_;
		int port_;
		int playerId_;
		string session_;


		public GameServerConnectorInfoSimulator(IGatewayServerAuthenticationRespondData data)
		{
			this.ip_ = data.targetServerIpAddr;
			this.port_ = data.targetServerPort;
			this.session_ = data.targetServerAccessToken;
			this.playerId_ = data.playerId;
		}
	}

	public class LoginRequestSimualtor : AGameLoginRequest
	{
		protected override void doLoginRequest()
		{
			NetworkEventMgr.Instance.RemoveListener(Proto.CSProtoID.RSP_PLAYER_LOGIN, RecPlayerLogin);
			NetworkEventMgr.Instance.AddListener(Proto.CSProtoID.RSP_PLAYER_LOGIN, RecPlayerLogin);
			NetWorkInstance.Instance.Login(this.playerId);
		}

		private void RecPlayerLogin(ProtoBuf.IExtensible unit)
		{
			NetworkEventMgr.Instance.RemoveListener(CSProtoID.RSP_PLAYER_LOGIN, RecPlayerLogin);
			RspPlayerLogin data = unit as RspPlayerLogin;
			if (data.resultID == (int) ErrorCode.SUCCESS)
			{
				onGameLoginRespond(new GameUserInfoSimulator(this.playerId, data));
			}
			else
			{
				string codeString = data.resultID.ToString();
				if (Enum.IsDefined(typeof(ErrorCode), data.resultID))
				{
					codeString = ((ErrorCode)data.resultID).ToString();
				}
				Debug.LogError(string.Format("Player Login Failed. login info : playerId={0}, errCode={1}", this.playerId, codeString));
			}
		}
	}

	public class GameUserInfoSimulator : AGameUserInfo
	{
		public GameUserInfoSimulator(int roleId, RspPlayerLogin data) : base(roleId, data) {}

		protected override void doRequestSetUserInfo()
		{
			NetworkEventMgr.Instance.RemoveListener(CSProtoID.RSP_SET_ROLE_INFO, RecSetRoleInfo);
			NetworkEventMgr.Instance.AddListener(CSProtoID.RSP_SET_ROLE_INFO, RecSetRoleInfo);
			NetWorkInstance.Instance.SetRoleInfo(nickName_, playerSex_);
		}

		private void RecSetRoleInfo(ProtoBuf.IExtensible unit)
		{
			RspSetRoleInfo data = unit as RspSetRoleInfo;
			if (data.resultID == (int)ErrorCode.SUCCESS)
			{
				//set userinfo succeed.
				isNewUser_ = false;
			}
			onRequestSetUserInfoRespond(data.resultID);
		}
	}

}
