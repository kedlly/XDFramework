
namespace Assets.CommunicationSystem.SubSystems.Connection
{

	#region command
	using TargetGatewayServerAuthenticate = Simulator.GatewayServerAuthenticateSimulator;
	using TargetGameServerConnector = Simulator.GameServerConnectorSimulator;
	#endregion

	#region command data
	using TargetGatewayAuthRequestData = Simulator.GatewayAuthRequestDataSimulator;
	using TargetGameServerConnectorInfo = Simulator.GameServerConnectorInfoSimulator;
	using System;

	#endregion


	//--------------------------------------------------------------------------------------------------------------------



	public class BridgeConnectionSubsystem : AConnectionSubsystem
	{
		IGatewayServerAuthenticate gatewayRequest_;

		public BridgeConnectionSubsystem() : base(new TargetGameServerConnector())
		{
			gatewayRequest_ = new TargetGatewayServerAuthenticate();
			gatewayRequest_.onConnected += onGatewayConnectionResult;
			gatewayRequest_.onResponded += onGatewayServerResponded;
		}

		

		void postGatewayAuthRequest()
		{
			if (gatewayRequest_ != null)
			{
				gatewayRequest_.connect(true);
			}
		}

		void postGatewayAuthRequest(IGatewayServerAuthenticationRequestData iData)
		{
			if (iData != null && gatewayReqData != iData)
			{
				gatewayReqData = iData;
			}
			if (gatewayRequest_ != null)
			{
				gatewayRequest_.connect(true);
			}
		}

		void onGatewayConnectionResult(int errorCode)
		{
			if (errorCode == 0)
			{
				gatewayRequest_.postAuthRequest(getRequestData());
			}
		}

		IGatewayServerAuthenticationRequestData gatewayReqData = null;

		private IGatewayServerAuthenticationRequestData getRequestData()
		{
			if (gatewayReqData == null)
			{
				gatewayReqData = new TargetGatewayAuthRequestData();
			}
			return gatewayReqData;
		}

		private void onGatewayServerResponded(IGatewayServerAuthenticationRespondData data)
		{
			if (data.errCode == (int)Proto.ErrorCode.SUCCESS)
			{
				UnityEngine.Debug.Log(
					string.Format(
						"Login server:{0}, port:{1}, accessToken:{2}, pid:{3}"
						, data.targetServerIpAddr
						, data.targetServerPort
						, data.targetServerAccessToken
						, data.playerId
					)
				);
				// go game server process
				gatewayRequest_.disconnect();
				if (gameServerConnector_ != null)
				{
					gameServerConnector_.connectInfo = new TargetGameServerConnectorInfo(data);
				}
				((IConnectionSubsystem)this).connectGameServer();
			}
			else
			{
				UnityEngine.Debug.Log(
					string.Format(
						"Login server Unknow. Error Code : {0}"
						, data.errCode
					)
				);
			}

		}

		public void disconnectGameServer()
		{
			if (this.gameServerConnector_ != null)
			{
				this.gameServerConnector_.disconnect();
			}
		}

		protected override void connectGameServer()
		{
			if (this.gameServerConnector_ != null)
			{
				this.gameServerConnector_.connect( true);
			}
			else
			{
				this.postGatewayAuthRequest();
			}
		}

		protected override void connectGameServer(IGatewayServerAuthenticationRequestData iData)
		{
			this.postGatewayAuthRequest(iData);
		}

	}
}
