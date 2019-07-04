using Unity.Entities;

namespace Messenging.Demo {

  public struct Plant : IComponentData {}

  public class PlantProxy : ProxyBase<Plant> {
    protected override Plant Convert() {
      return new Plant();
    }
  }

}
