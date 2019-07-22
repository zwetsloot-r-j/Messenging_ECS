using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;
using Messenging;

namespace Messenging.Demo {

  class MoveSystem : MessageSystem<MovePayload> {

    [BurstCompile]
    struct MoveJob : IJobForEach<Address, Translation> {
      [ReadOnly]
      public EntityCommandBuffer CommandBuffer;

      [ReadOnly]
      [DeallocateOnJobCompletion]
      public NativeArray<Receiver> Receivers;

      [ReadOnly]
      [DeallocateOnJobCompletion]
      public NativeArray<MovePayload> Payloads;

      public void Execute(
        [ReadOnly] ref Address address,
        ref Translation translation
      ) {
        Receiver receiver;
        MovePayload payload;
        if (Message.FindPayloadForReceiver<MovePayload>(Receivers, Payloads, address, out payload, out receiver)) {
          translation.Value += payload.Translation;
        }
      }
    }

    protected override JobHandle HandleMessage(JobHandle handle, EntityCommandBuffer commandBuffer) {
      var moveJob = new MoveJob {
        CommandBuffer = commandBuffer,
        Receivers = Receivers,
        Payloads = Payloads,
      };

      return moveJob.Schedule(this, handle);
    }
  }

}
