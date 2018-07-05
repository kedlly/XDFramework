

using System;
using Assets.LoginSystem.UIUtils;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.CommunicationSystem.Simulator.UIUtils
{
	using TargetGatewayAuthRequestData = GatewayAuthRequestDataSimulator;
	using TargetAuthServer = GatewayServerAuthenticateSimulator;

	public class LoginUISimulator : ILoginUI
	{
		const string KEY_USERNAME = "username";
		const string KEY_PASSWORD = "password";
		const string KEY_ANONYMOUS_KEY = "anonymous_KEY";

		IGatewayServerAuthenticate targetAuthServer = new TargetAuthServer();

		Dictionary<string, string> kvCollection = new Dictionary<string, string>();
		string ILoginUI.password
		{
			get
			{
				if (this.kvCollection.ContainsKey(KEY_USERNAME))
				{
					return kvCollection[KEY_USERNAME];
				}
				return null;
			}
			set
			{
				kvCollection[KEY_USERNAME] = value;
			}
		}

		string ILoginUI.username
		{
			get
			{
				if (this.kvCollection.ContainsKey(KEY_PASSWORD))
				{
					return kvCollection[KEY_PASSWORD];
				}
				return null;
			}
			set
			{
				kvCollection[KEY_PASSWORD] = value;
			}
		}

		event LoginUIProcessResult onProcessLoginRequestData_;
		event LoginUIAnonymousUserRequestResult onAnonymousCredits_;

		event LoginUIProcessResult ILoginUI.onLoginResult
		{
			add
			{
				onProcessLoginRequestData_ += value;
			}

			remove
			{
				onProcessLoginRequestData_ -= value;
			}
		}

		event LoginUIAnonymousUserRequestResult ILoginUI.onAnonymousCredits
		{
			add
			{
				onAnonymousCredits_ += value;
			}

			remove
			{
				onAnonymousCredits_ -= value;
			}
		}

		void ILoginUI.getAnonymousIdFromLoginServer(bool isAsync)
		{
			if (targetAuthServer != null)
			{
				targetAuthServer.onConnected += delegate (int errorCode)
				{
					if (targetAuthServer != null && errorCode == 0)
					{
						targetAuthServer.onResponded += delegate (IGatewayServerAuthenticationRespondData iData)
						{
							targetAuthServer.disconnect();
							if (!string.IsNullOrEmpty(iData.targetServerAccessToken))
							{
								kvCollection[KEY_ANONYMOUS_KEY] = iData.targetServerAccessToken;
								if (onAnonymousCredits_ != null)
								{
									onAnonymousCredits_(iData.targetServerAccessToken);
								}
							}
						};
						targetAuthServer.postAuthRequest(null);
					}
				};
				targetAuthServer.connect(isAsync);
			}
		}

		void ILoginUI.login(bool isAnonymous, bool isAsyncLogin)
		{
			if (isAnonymous)
			{
				if (kvCollection.ContainsKey(KEY_ANONYMOUS_KEY) && !string.IsNullOrEmpty(kvCollection[KEY_ANONYMOUS_KEY]))
				{
					loginWithAnonymousUser();
				}
				else
				{
					((ILoginUI)this).getAnonymousIdFromLoginServer(isAsyncLogin);
				}
			}
			else
			{
				loginWithUsernameAndPassword();
			}
		}

		void loginWithAnonymousUser()
		{
			if (onProcessLoginRequestData_ != null)
			{
				onProcessLoginRequestData_(new TargetGatewayAuthRequestData(kvCollection[KEY_ANONYMOUS_KEY]));
			}
		}

		void loginWithUsernameAndPassword()
		{
			if (string.IsNullOrEmpty(kvCollection[KEY_USERNAME]) || string.IsNullOrEmpty(kvCollection[KEY_PASSWORD]))
			{
				return;
			}
			if (onProcessLoginRequestData_ != null)
			{
				onProcessLoginRequestData_(new TargetGatewayAuthRequestData(kvCollection[KEY_USERNAME], kvCollection[KEY_PASSWORD]));
			}
		}

		void ILoginUI.restoreUserCredits()
		{
			kvCollection[KEY_USERNAME] = PlayerPrefs.GetString(KEY_USERNAME, null);
			kvCollection[KEY_PASSWORD] = PlayerPrefs.GetString(KEY_PASSWORD, null);
			kvCollection[KEY_ANONYMOUS_KEY] = PlayerPrefs.GetString(KEY_ANONYMOUS_KEY, null);
		}

		void ILoginUI.storeUserCredits()
		{
			foreach (var item in kvCollection)
			{
				PlayerPrefs.SetString(item.Key, item.Value);
			}
		}

		public LoginUISimulator()
		{
			kvCollection[ KEY_USERNAME ] = null;
			kvCollection[ KEY_PASSWORD ]= null;
			kvCollection[ KEY_ANONYMOUS_KEY ] = null;

			onAnonymousCredits_ += delegate (string data)
			{
				kvCollection[KEY_ANONYMOUS_KEY] = data;
				loginWithAnonymousUser();
			};
		}
	}
}
