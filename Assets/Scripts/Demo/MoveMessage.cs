using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using System;
using System.Runtime.Serialization;

namespace Messenging.Demo {

  public struct MovePayload : IPayload {
    public float3 Translation;
  }

  [DataContract]
  public class MovePayloadConverter : IPayloadConverter {
    [DataMember]
    private Vector3 Translation;

    public IPayloadConverter Convert(IPayload payload) {
      var movePayload = (MovePayload)payload;
      Translation = new Vector3(
        movePayload.Translation.x,
        movePayload.Translation.y,
        movePayload.Translation.z
      );

      return this;
    }

    public IPayload Revert() {
      return new MovePayload {
        Translation = new float3(
          Translation.x,
          Translation.y,
          Translation.z
        )
      };
    }
  }

  [DataContract]
  public class MoveMessage : Message {
    public MoveMessage() {
      converter = new MovePayloadConverter();
    }

    public override void Send(EntityCommandBuffer commandBuffer) {
      DoSend<MovePayload>(commandBuffer);
    }
  }

}
