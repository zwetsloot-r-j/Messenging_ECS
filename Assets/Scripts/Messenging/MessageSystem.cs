using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using System;
using UnityEngine;

namespace Messenging {

  public abstract class MessageSystem<TPayload> : JobComponentSystem where TPayload : struct, IPayload {
    protected BeginInitializationEntityCommandBufferSystem commandBufferSystem;
    protected EntityQuery messageQuery;

    protected NativeArray<Receiver> Receivers {
      get {
        return messageQuery.ToComponentDataArray<Receiver>(Allocator.TempJob);
      }
    }

    protected NativeArray<TPayload> Payloads {
      get {
        return messageQuery.ToComponentDataArray<TPayload>(Allocator.TempJob);
      }
    }

    [BurstCompile]
    struct DestroyJob : IJobForEachWithEntity<TPayload, Receiver> {
      [ReadOnly]
      public EntityCommandBuffer CommandBuffer;

      public void Execute(Entity entity, int i, [ReadOnly] ref TPayload payload, [ReadOnly] ref Receiver receiver) {
        CommandBuffer.DestroyEntity(entity);
      }
    }

    protected override void OnCreate() {
      messageQuery = GetEntityQuery(typeof(Receiver), typeof(TPayload));
      commandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
      Init();
    }

    protected override JobHandle OnUpdate(JobHandle handle) {
      var commandBuffer = commandBufferSystem.CreateCommandBuffer();

      var destroyJob = new DestroyJob {
        CommandBuffer = commandBuffer,
      };

      handle = HandleMessage(handle, commandBuffer);
      handle = destroyJob.Schedule(this, handle);
      commandBufferSystem.AddJobHandleForProducer(handle);

      return handle;
    }

    protected virtual void Init() {}

    protected abstract JobHandle HandleMessage(JobHandle handle, EntityCommandBuffer commandBuffer);
  }

}
