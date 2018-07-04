using System;

namespace Framework.Services.Positioning
{
	[Serializable]
	public class Coordinate
	{
		public int x;
		public int y;
		public int z;
	}

	[Serializable]
	public class PositioningInfo
	{
		public int id;
		public Coordinate coordinate;
	}
}
