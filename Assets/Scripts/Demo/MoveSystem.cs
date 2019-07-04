using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;
using Messenging;

namespace Messenging.Demo {

  class MoveSystem : JobComponentSystem {
    EntityQuery moveMessageQuery;
    BeginInitializationEntityCommandBufferSystem commandBufferSystem;

    [BurstCompile]
    struct TranslateJob : IJobForEach<Translation, Address> {
      [ReadOnly]
      [DeallocateOnJobCompletion]
      public NativeArray<Receiver> Receivers;

      [ReadOnly]
      [DeallocateOnJobCompletion]
      public NativeArray<MovePayload> MovePayloads;

      public void Execute(ref Translation translation, [ReadOnly] ref Address address) {
        for (var i = 0; i < Receivers.Length; i++) {
          if (Receivers[i].Id == address.Id) {
            translation.Value += MovePayloads[i].Translation;
            break;
          }
        }
      }
    }

    [BurstCompile]
    struct DestroyJob : IJobForEachWithEntity<MovePayload, Receiver> {
      [ReadOnly]
      public EntityCommandBuffer CommandBuffer;

      public void Execute(Entity entity, int index, [ReadOnly] ref MovePayload move, [ReadOnly] ref Receiver receiver) {
        CommandBuffer.DestroyEntity(entity);
      }
    }

    protected override void OnCreate() {
      moveMessageQuery = GetEntityQuery(typeof(Receiver), typeof(MovePayload));
      commandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle handle) {
      var destroyJob = new DestroyJob {
        CommandBuffer = commandBufferSystem.CreateCommandBuffer()
      };

      var translateJob = new TranslateJob {
        Receivers = moveMessageQuery.ToComponentDataArray<Receiver>(Allocator.TempJob),
        MovePayloads = moveMessageQuery.ToComponentDataArray<MovePayload>(Allocator.TempJob)
      };

      handle = translateJob.Schedule(this, handle);
      handle = destroyJob.Schedule(this, handle);
      commandBufferSystem.AddJobHandleForProducer(handle);

      return handle;
    }
  }

}
