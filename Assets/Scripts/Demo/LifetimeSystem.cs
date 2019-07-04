using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using Messenging;

namespace Messenging.Demo {

  class LifetimeSystem : JobComponentSystem {
    BeginInitializationEntityCommandBufferSystem commandBufferSystem;

    [BurstCompile]
    struct DespawnJob : IJobForEachWithEntity<Lifetime> {
      [ReadOnly]
      public EntityCommandBuffer CommandBuffer;

      [ReadOnly]
      public float DeltaTime;

      public void Execute(Entity entity, int i, ref Lifetime lifetime) {
        lifetime.CurrentLifetime += DeltaTime;

        if (lifetime.CurrentLifetime >= lifetime.MaxLifetime) {
          CommandBuffer.DestroyEntity(entity);
        }
      }
    }

    protected override void OnCreate() {
      commandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle handle) {
      var commandBuffer = commandBufferSystem.CreateCommandBuffer();

      var despawnJob = new DespawnJob {
        CommandBuffer = commandBuffer,
        DeltaTime = Time.deltaTime,
      };

      handle = despawnJob.Schedule(this, handle);
      commandBufferSystem.AddJobHandleForProducer(handle);

      return handle;
    }

  }

}
