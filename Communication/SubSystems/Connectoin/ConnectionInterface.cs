using System;

namespace Assets.CommunicationSystem.SubSystems.Connection
{
	public delegate void ConnectGameServerResult();
	public interface IConnectionSubsystem
	{
		IGameServerConnectorInfo connectionInfo { get; }
		void connectGameServer();
		void connectGameServer(IGatewayServerAuthenticationRequestData iData);
		void reconnectGameServer();
		event ConnectGameServerResult connectionRespond;
		event ConnectGameServerResult reconnectionRespond;
	}


	public abstract class AConnectionSubsystem : IConnectionSubsystem
	{

		protected IGameServerConnector gameServerConnector_;

		IGameServerConnectorInfo IConnectionSubsystem.connectionInfo
		{
			get
			{
				return gameServerConnector_.connectInfo;
			}
		}

		protected event ConnectGameServerResult connectionRespond_;

		event ConnectGameServerResult IConnectionSubsystem.connectionRespond
		{
			add
			{
				connectionRespond_ -= value;
				connectionRespond_ += value;
			}

			remove
			{
				connectionRespond_ -= value;
			}
		}

		protected event ConnectGameServerResult reconnectionRespond_;
		event ConnectGameServerResult IConnectionSubsystem.reconnectionRespond
		{
			add
			{
				reconnectionRespond_ -= value;
				reconnectionRespond_ += value;
			}

			remove
			{
				reconnectionRespond_ -= value;
			}
		}

		void IConnectionSubsystem.connectGameServer()
		{
			this.connectGameServer();
		}
		protected abstract void connectGameServer();
		protected abstract void connectGameServer(IGatewayServerAuthenticationRequestData iData);
		void IConnectionSubsystem.connectGameServer(IGatewayServerAuthenticationRequestData iData)
		{
			connectGameServer(iData);
		}

		void IConnectionSubsystem.reconnectGameServer()
		{
			if (this.gameServerConnector_ != null)
			{
				this.gameServerConnector_.reconnect(true);
			}
		}

		public AConnectionSubsystem(IGameServerConnector iGSC)
		{
			gameServerConnector_ = iGSC; //
			gameServerConnector_.onConnected += onGameServerConnected;
			gameServerConnector_.onReconnected += onGameServerReconnected;
		}

		void onGameServerConnected(int errorCode)
		{
			if (connectionRespond_ != null)
			{
				connectionRespond_();
			}
		}

		void onGameServerReconnected(int errorCode)
		{
			if (reconnectionRespond_ != null)
			{
				reconnectionRespond_();
			}
		}
	}
}