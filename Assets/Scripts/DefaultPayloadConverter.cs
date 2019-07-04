using System.Runtime.Serialization;

namespace Messenging {

  [DataContract]
  public class DefaultPayloadConverter : IPayloadConverter {

    [DataMember]
    private IPayload payload;

    public IPayloadConverter Convert(IPayload payload) {
      this.payload = payload;
      return this;
    }

    public IPayload Revert() {
      return payload;
    }
  }

}
