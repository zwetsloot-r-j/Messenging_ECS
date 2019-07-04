using Unity.Entities;

namespace Messenging.Demo {

  public struct Lifetime : IComponentData {
    public float CurrentLifetime;
    public float MaxLifetime;
  }

  public class LifetimeProxy : ProxyBase<Lifetime> {
    public float MaxLifetime = 0;

    protected override Lifetime Convert() {
      return new Lifetime {
        CurrentLifetime = 0,
        MaxLifetime = this.MaxLifetime,
      };
    }
  }

}
