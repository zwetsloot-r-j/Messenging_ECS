using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using System;
using System.Runtime.Serialization;

namespace Messenging.Demo {

  public struct ShootPayload : IPayload {}

  [DataContract]
  public class ShootMessage : Message {
    public override void Send(EntityCommandBuffer commandBuffer) {
      DoSend<ShootPayload>(commandBuffer);
    }
  }

}
