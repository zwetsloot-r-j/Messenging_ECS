using Unity.Entities;

namespace Messenging.Demo {

  public struct Projectile : IComponentData {}

  public class ProjectileProxy : ProxyBase<Projectile> {
    protected override Projectile Convert() {
      return new Projectile();
    }
  }

}
