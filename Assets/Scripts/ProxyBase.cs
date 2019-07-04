using UnityEngine;
using Unity.Entities;
using System;

namespace Messenging {

  public abstract class ProxyBase<T> : MonoBehaviour, IConvertGameObjectToEntity where T : struct, IComponentData {
    protected abstract T Convert();

    public void Convert(Entity entity, EntityManager manager, GameObjectConversionSystem conversionSystem) {
      manager.AddComponentData(entity, Convert());
    }
  }

}
