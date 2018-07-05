using Assets.CommunicationSystem.SubSystems.Connection;
using Assets.CommunicationSystem.SubSystems.Login;


namespace Assets.CommunicationSystem
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
