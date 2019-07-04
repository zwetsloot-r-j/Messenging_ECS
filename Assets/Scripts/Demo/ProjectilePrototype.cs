using Unity.Entities;

namespace Messenging.Demo {

  public struct ProjectilePrototype : IComponentData {
    public Entity Prototype;
    public int SpawnCount;
  }

}
