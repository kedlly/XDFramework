



using System;

namespace Assets.CommunicationSystem.SubSystems.Login
{
	public interface ILoginSubSystem
	{
		void login(int playerId);
		event onGameLoginResult onLoginRespond;
		IUserInfo UserInfo { get; }
	}

	public abstract class ALoginSubSystem : ILoginSubSystem
	{
		protected ILoginRequest loginRequest_ { get; private set; }

		private IUserInfo userInfoHandler_;
		IUserInfo ILoginSubSystem.UserInfo
		{
			get
			{
				return userInfoHandler_;
			}
		}

		event onGameLoginResult ILoginSubSystem.onLoginRespond
		{
			add
			{
				loginRequest_.onLoginRespond -= value;
				loginRequest_.onLoginRespond += value;
			}

			remove
			{
				loginRequest_.onLoginRespond -= value;
			}
		}

		void ILoginSubSystem.login(int playerId)
		{
			loginRequest_.login(playerId);
		}

		public ALoginSubSystem(ILoginRequest ilr)
		{
			loginRequest_ = ilr;
			loginRequest_.onLoginRespond -= onLoginRespondHandler;
			loginRequest_.onLoginRespond += onLoginRespondHandler;
			userInfoHandler_ = null;
		}

		private void onLoginRespondHandler(IUserInfo iInfo)
		{
			userInfoHandler_ = iInfo;
		}
	}
}
