using System;
using UnityEngine.UI;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using GLTFast;
using System.Threading.Tasks;

namespace Utils
{
	public static class GLTFSpawner
	{
		public static async Task<GameObject> SpawnGLTF(Transform root, Wezit.Node wzData)
		{
			string source = "";
			if (wzData != null)
			{
				await wzData.GetRelations(wzData);
				foreach (Wezit.Relation relation in wzData.Relations)
				{
					if (relation.relation == Wezit.RelationName.SHOW_3D_MODEL)
					{
						source = relation.GetAssetSourceByTransformation(WezitSourceTransformation.default_base);
					}
				}
			}
			else
            {
				return null;
            }

			if(string.IsNullOrEmpty(source))
            {
				return null;
            }

			GltfImport gltfImport = new GltfImport();
			await gltfImport.Load(source);
			GameObjectInstantiator instantiator = new GameObjectInstantiator(gltfImport, root);
			bool success = await gltfImport.InstantiateMainSceneAsync(instantiator);
			if (success)
			{
				// Get the SceneInstance to access the instance's properties
				GameObjectSceneInstance sceneInstance = instantiator.SceneInstance;

				// Play the default (i.e. the first) animation clip
				Animation legacyAnimation = sceneInstance.LegacyAnimation;
				if (legacyAnimation != null)
				{
					legacyAnimation.Play();
				}

				return instantiator.SceneTransform.gameObject;
			}
			else
            {
				return null;
            }
		}
	}
}
