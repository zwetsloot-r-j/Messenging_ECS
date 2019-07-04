using Unity.Entities;
using UnityEngine;
using System.Collections.Generic;

namespace Messenging.Demo {

  public class ProjectilePrototypeProxy : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity {

    public GameObject Prefab;
    public int SpawnCount = 1;

    public void DeclareReferencedPrefabs(List<GameObject> gameObjects) {
      gameObjects.Add(Prefab);
    }

    public void Convert(Entity entity, EntityManager entityManager, GameObjectConversionSystem conversionSystem) {
      var projectilePrototype = new ProjectilePrototype {
        Prototype = conversionSystem.GetPrimaryEntity(Prefab),
        SpawnCount = this.SpawnCount,
      };

      entityManager.AddComponentData(entity, projectilePrototype);
    }
  }

}
