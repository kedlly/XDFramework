using Assets.CommunicationSystem.SubSystems.Connection;
using Framework.System.Communication.Login;


namespace Framework.System.Communication
{
	public class CommunicationSystem : ToSingleton<CommunicationSystem>
	{
		IConnectionSubsystem connectoinSubsystem_ = new BridgeConnectionSubsystem();
		ILoginSubSystem loginSubSystem_ = new NormalLoginSubSystem();

		private CommunicationSystem()
		{
			connectoinSubsystem_.connectionRespond += delegate ()
			{

			};

			connectoinSubsystem_.reconnectionRespond += delegate ()
			{

			};
		}

		public IConnectionSubsystem connectoinSubSystem
		{
			get
			{
				return connectoinSubsystem_;
			}
		}

		public ILoginSubSystem loginSubSystem
		{
			get
			{
				return loginSubSystem_;
			}
		}




	}
}
