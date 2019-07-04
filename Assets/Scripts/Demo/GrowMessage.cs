using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using System;
using System.Runtime.Serialization;

namespace Messenging.Demo {

  public struct GrowPayload : IPayload {}

  [DataContract]
  public class GrowMessage : Message {
    public override void Send(EntityCommandBuffer commandBuffer) {
      DoSend<GrowPayload>(commandBuffer);
    }
  }

}
