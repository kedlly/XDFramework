using UnityEngine;
using Framework.Library.Singleton;
using Framework.Utils.Extensions;

namespace Framework.Core
{
	//[PathInHierarchyAttribute("/[Game]/Systems"), DisallowMultipleComponent]
	public sealed class AssetsManager : ToSingletonBehavior<AssetsManager>
	{
		AssetBundleLoader loader = null;
		public void Initalize()
		{
			//"Assets/StreamingAssets/StreamingAssets", "client.pfb"
			loader = new AssetBundleLoader("Assets/StreamingAssets/client.pfb");
			//loader.AssetName = "Client";
		}

        private void Update()
        {
            loader.CheckOperation();
			loader.LoadAsync();
		}
	}
}
