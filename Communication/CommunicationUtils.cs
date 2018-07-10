


using System;
using UnityEngine;

namespace Framework.System.Communication
{
	public abstract class AGatewayServerAuthenticate : IGatewayServerAuthenticate
	{

		///Json code format example
		///{
		///		"errCode":0
		///		"ip":"192.168.0.1",
		///		"port":3650,
		///		"playerId":10010,
		///		"accessToken":"aSBsb3ZlIG15IGZhbWlseQ==",
		///}
		public class GatewayServerAuthenticationRespondData : IGatewayServerAuthenticationRespondData
		{
			public int errCode;
			public string ip;
			public int port;
			public int playerId;
			public string accessToken;
			string IGatewayServerAuthenticationRespondData.targetServerIpAddr { get { return this.ip; } }
			int IGatewayServerAuthenticationRespondData.targetServerPort { get { return this.port; } }
			string IGatewayServerAuthenticationRespondData.targetServerAccessToken { get { return this.accessToken; } }
			int IGatewayServerAuthenticationRespondData.errCode { get { return this.errCode; } }
			int IGatewayServerAuthenticationRespondData.playerId { get { return this.playerId; } }
			public static GatewayServerAuthenticationRespondData FromJson(string json)
			{
				return UnityEngine.JsonUtility.FromJson<GatewayServerAuthenticationRespondData>(json);
			}
		}

		event onGatewayServerResponded onResponded_;
		event onGatewayServerConnected onConnected_;

		event onGatewayServerResponded IGatewayServerAuthenticate.onResponded
		{
			add
			{
				onResponded_ -= value;
				onResponded_ += value;
			}

			remove
			{
				onResponded_ -= value;
			}
		}

		event onGatewayServerConnected IGatewayServerAuthenticate.onConnected
		{
			add
			{
				onConnected_ -= value;
				onConnected_ += value;
			}

			remove
			{
				onConnected_ -= value;
			}
		}

		void IGatewayServerAuthenticate.postAuthRequest(IGatewayServerAuthenticationRequestData dataItf)
		{
			if (dataItf == null)
			{
				dataItf = getDefaultRequestData();
			}
			string uniCode = dataItf.uniqueClientCode;
			int platformId = dataItf.currentPlatformID;
			this.send(uniCode, platformId);
		}

		protected abstract void send(string uniCode, int platformId);

		protected void pumpRespondData(IGatewayServerAuthenticationRespondData data)
		{
			if (onResponded_ != null)
			{
				onResponded_(data);
			}
		}

		protected abstract IGatewayServerAuthenticationRequestData getDefaultRequestData();

		void IGatewayServerAuthenticate.connect(bool isAsync)
		{
			onConnected_ -= this.onConnectionResult;
			onConnected_ += this.onConnectionResult;
			if (isAsync)
			{
				asyncConnect();
			}
			else
			{
				syncConnect();
			}
		}

		public void triggerConnectionEvent(int errorCode)
		{
			if (this.onConnected_ != null)
			{
				this.onConnected_(errorCode);
			}
		}

		protected abstract void syncConnect();

		protected abstract void asyncConnect();

		protected abstract void onConnectionResult(int errorCode);

		protected abstract void disconnect();

		void IGatewayServerAuthenticate.disconnect()
		{
			disconnect();
		}

	}
	public abstract class AGameServerConnector : IGameServerConnector
	{

		event onGameServerConnected onConnected_;
		event onGameServerConnected onReonnected_;

		IGameServerConnectorInfo connectInfo_;
		IGameServerConnectorInfo IGameServerConnector.connectInfo
		{
			get
			{
				return connectInfo_;
			}

			set
			{
				connectInfo_ = value;
			}
		}

		event onGameServerConnected IGameServerConnector.onConnected
		{
			add
			{
				onConnected_ += value;
			}

			remove
			{
				onConnected_ -= value;
			}
		}

		event onGameServerConnected IGameServerConnector.onReconnected
		{
			add
			{
				onReonnected_ += value;
			}

			remove
			{
				onReonnected_ -= value;
			}
		}

		protected void triggerConnectionEvent(int errorCode)
		{
			if (this.onConnected_ != null)
			{
				this.onConnected_(errorCode);
			}
		}

		protected void triggerReconnectionEvent(int errorCode)
		{
			if (this.onReonnected_ != null)
			{
				this.onReonnected_(errorCode);
			}
		}

		protected abstract void syncConnect(IGameServerConnectorInfo connectInfo);

		protected abstract void asyncConnect(IGameServerConnectorInfo connectInfo);

		protected abstract void onConnectionResult(int errorCode);

		protected abstract void syncReonnect(IGameServerConnectorInfo connectInfo);

		protected abstract void asyncReconnect(IGameServerConnectorInfo connectInfo);

		protected abstract void onReconnectionResult(int errorCode);

		public abstract void disconnect();

		void IGameServerConnector.connect(bool isAsync)
		{
			onConnected_ -= this.onConnectionResult;
			onConnected_ += this.onConnectionResult;
			if (isAsync)
			{
				asyncConnect(connectInfo_);
			}
			else
			{
				syncConnect(connectInfo_);
			}
		}

		void IGameServerConnector.reconnect(bool isAsync)
		{
			onReonnected_ -= this.onConnectionResult;
			onReonnected_ += this.onConnectionResult;
			if (isAsync)
			{
				asyncConnect(connectInfo_);
			}
			else
			{
				syncConnect(connectInfo_);
			}
		}

		void IGameServerConnector.disconnect()
		{
			this.disconnect();
		}
	}
	public abstract class AGameLoginRequest : ILoginRequest
	{
		event onGameLoginResult ILoginRequest.onLoginRespond
		{
			add
			{
				onLoginRespond_ += value;
			}

			remove
			{
				onLoginRespond_ -= value;
			}
		}

		event onGameLoginResult onLoginRespond_;

		protected int playerId { get; set; }

		void ILoginRequest.login(int playerId)
		{
			this.playerId = playerId;
			doLoginRequest();
		}

		protected abstract void doLoginRequest();
		protected void onGameLoginRespond(IUserInfo info)
		{
			if (onLoginRespond_ != null)
			{
				onLoginRespond_(info);
			}
		}
	}
	public abstract class AGameUserInfo : IUserInfo
	{
		string IUserInfo.nickName
		{
			get
			{
				return nickName_;
			}
			set
			{
				if (isNewUser_)
				{
					nickName_ = value;
					isNicknameOrGenderHasBeenSet_ = true;
				}
			}
		}

		SexType IUserInfo.gender
		{
			get
			{
				return playerSex_;
			}
			set
			{
				if (isNewUser_)
				{
					playerSex_ = value;
					isNicknameOrGenderHasBeenSet_ = true;
				}
			}
		}

		bool IUserInfo.isNewUser
		{
			get
			{
				return isNewUser_;
			}
		}

		int IUserInfo.roleId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		protected string nickName_;
		protected SexType playerSex_;
		protected int roleId_;

		private bool isNicknameOrGenderHasBeenSet_ = false;
		protected bool isNewUser_ = true;

		public AGameUserInfo(int roleId, Proto.RspPlayerLogin data)
		{
			roleId_ = roleId;
			isNewUser_ = data.is_new_user;
			nickName_ = data.name;
			switch (data.role_sex)
			{
			case Proto.PlayerSex.PLAYERSEX_BOY:
				playerSex_ = SexType.MALE;
				break;
			case Proto.PlayerSex.PLAYERSEX_GIRL:
				playerSex_ = SexType.FEMALE;
				break;
			default:
				playerSex_ = SexType.NONE;
				break;
			}
		}

		private event SetUserRoleInfoRespond onSetUserRoleInfo_;

		event SetUserRoleInfoRespond IUserInfo.onSetUserRoleInfo
		{
			add
			{
				onSetUserRoleInfo_ -= value;
				onSetUserRoleInfo_ += value;
			}

			remove
			{
				onSetUserRoleInfo_ -= value;
			}
		}

		void IUserInfo.requestSetUserRoleInfo()
		{
			if (isNewUser_ && isNicknameOrGenderHasBeenSet_)
			{
				doRequestSetUserInfo();
			}
			else
			{
				Debug.Log("only new user can set nickname and gender.");
			}
		}

		protected abstract void doRequestSetUserInfo();
		protected void onRequestSetUserInfoRespond(int errorCode)
		{
			if (onSetUserRoleInfo_ != null)
			{
				onSetUserRoleInfo_(errorCode);
			}
		}
	}
}