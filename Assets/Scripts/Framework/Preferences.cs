using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
	public interface IPreferences
	{
		void Register(IPreferenceSettings ps);
		IPreferenceSettings QueryPreference(Type type);
		void SaveAll();
		T QueryPreference<T>() where T : IPreferenceSettings;
	}

	public interface IPreferenceSettings
	{
		event Action OnChanged;
		bool AutoSave { get; set; }
		void Load();
		void Save();
	}

	class GamePreferences : IPreferences
	{
		public IPreferenceSettings QueryPreference(Type type)
		{
			return _settings.ContainsKey(type) ? _settings[type] : null;
		}

		public T QueryPreference<T>() where T : IPreferenceSettings
		{
			return (T)QueryPreference(typeof(T));
		}

		public void Register(IPreferenceSettings ps)
		{
			_settings[ps.GetType()] = ps;
		}

		void IPreferences.SaveAll()
		{
			foreach (var v in _settings.Values)
			{
				v.Save();
			}
		}

		Dictionary<Type, IPreferenceSettings> _settings;


		public GamePreferences()
		{
		}

		public GamePreferences(IPreferenceSettings[] settings) : this()
		{
			foreach (var setting in settings)
			{
				this.Register(setting);
			}
			
		}

		public static GamePreferences operator+ (GamePreferences obj, IPreferenceSettings setting)
		{
			obj.Register(setting);
			return obj;
		}
	}

	class VolumeSettings : IPreferenceSettings
	{
		public bool AutoSave { get ; set; }

		event Action IPreferenceSettings.OnChanged
		{
			add
			{
				throw new NotImplementedException();
			}

			remove
			{
				throw new NotImplementedException();
			}
		}

		void IPreferenceSettings.Load()
		{
			throw new NotImplementedException();
		}

		void IPreferenceSettings.Save()
		{
			throw new NotImplementedException();
		}
	}

}
