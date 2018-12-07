
using UnityEngine;

namespace Framework.Core.DataStruct
{

	public class SpringValue
	{
		public float Value { get { return Invert ? -1 * _value : _value; } }
		public bool Positive { get; set; }
		public bool Negative { get; set; }
		public float Gravity { get; set; } // = 3
		public float Dead { get; set; } // 0.001
		public float Sensitivity { get; set; }
		public bool Invert { get; set; }

		private float _value = 0.0f;

		public SpringValue()
		{
			Positive = false;
			Negative = false;
			Dead = 0.001f;
			Gravity = Sensitivity = 1000;
			Invert = false;
		}

		public void Update()
		{
			float deltaStep = 0;
			if ((!Positive && !Negative || Positive && Negative) && _value != 0)
			{
				if (Mathf.Abs(_value) > Dead)
				{
					deltaStep += _value > 0 ? -Gravity : Gravity;
				}
				else
				{
					_value = 0;
				}
			}
			
			if (Positive)
			{
				deltaStep += Sensitivity;
			}
			if (Negative)
			{
				deltaStep += -Sensitivity;
			}
			if (deltaStep != 0)
			{
				_value += deltaStep * Time.deltaTime;
				_value = Mathf.Clamp(_value, -1, 1);
			}
		}
	}
}
