using UnityEngine;
using Framework.Library.Singleton;
using Framework.Utils.Extensions;

namespace Framework.Core
{
	[PathInHierarchyAttribute("/[Game]/Systems"), DisallowMultipleComponent]
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
            /*			loader.CheckOperation();
                        if (Input.GetKeyDown(KeyCode.V))
                        {
                            //Debug.Log(ApplicationManager.Instance);
                            //LoadAsset();
                            if (loader.State == OperationState.Ready )
                            {
                                loader.LoadAsync();
                            }
                            else
                            {
            <<<<<<< Updated upstream
                                if (OnLoadAsset != null)
                                {
                                    OnLoadAsset(0f, false, null);
                                }
                                T obj = bundle.LoadAsset<T>(assetName);
                                if (OnLoadAsset != null)
                                {
                                    OnLoadAsset(1f, true, obj);
                                }
                                // 					if (obj == null)
                                // 					{
                                // 						Debug.LogErrorFormat("Cant load Asset with name {0} in AssetBundle : {1}, async = false", assetName, abPath);
                                // 						yield return null;
                                // 					}
                                // 					AddToBuffer(assetName, obj);
                            }
                        }
                        bundle.Unload(false);
                    }

                    //public void LoadAllAssetAsync<>
            =======
                                loader.AssetLoaderProxy.LoadAssetAsync("Client");
            >>>>>>> Stashed changes

                            }
                        }
                        if (Input.GetKeyDown(KeyCode.P))
                        {
                            var obj = loader.AssetLoaderProxy.GetAsset<UnityEngine.Object>("Client");
                            if (obj != null)
                            {
                                Instantiate(obj);
                            }
                            else
                            {
                                loader.AssetLoaderProxy.LoadAssetAsync("Client");
                            }
                        }
                        Debug.Log(loader.Progress);
                    }
                    */
        }
	}
}