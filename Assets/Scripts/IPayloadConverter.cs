namespace Messenging {
  public interface IPayloadConverter {
    IPayloadConverter Convert(IPayload payload);
    IPayload Revert();
  }
}
