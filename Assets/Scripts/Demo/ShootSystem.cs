using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Messenging;

namespace Messenging.Demo {

  class ShootSystem : JobComponentSystem {
    private static int lastId = 101;

    BeginInitializationEntityCommandBufferSystem commandBufferSystem;
    EntityQuery shootMessageQuery;

    //[BurstCompile]
    struct SpawnJob : IJobForEach<Address, ProjectilePrototype, LocalToWorld> {
      [ReadOnly]
      public EntityCommandBuffer CommandBuffer;

      [ReadOnly]
      [DeallocateOnJobCompletion]
      public NativeArray<Receiver> Receivers;

      public void Execute(
        [ReadOnly] ref Address address,
        [ReadOnly] ref ProjectilePrototype prototype,
        [ReadOnly] ref LocalToWorld localToWorld
      ) {
        for (var i = 0; i < Receivers.Length; i++) {
          if (Receivers[i].Id == address.Id) {
            for (var x = 0; x < prototype.SpawnCount; x++) {
              var projectile = CommandBuffer.Instantiate(prototype.Prototype);
              var position = math.transform(localToWorld.Value, new float3(0, 0, 0));
              CommandBuffer.SetComponent(projectile, new Translation { Value = position });
              CommandBuffer.SetComponent(projectile, new Address { Id = lastId });
              lastId++;
            }
            break;
          }
        }
      }
    }

    [BurstCompile]
    struct DestroyJob : IJobForEachWithEntity<ShootPayload, Receiver> {
      [ReadOnly]
      public EntityCommandBuffer CommandBuffer;

      public void Execute(Entity entity, int i, [ReadOnly] ref ShootPayload payload, [ReadOnly] ref Receiver receiver) {
        CommandBuffer.DestroyEntity(entity);
      }
    }

    protected override void OnCreate() {
      shootMessageQuery = GetEntityQuery(typeof(Receiver), typeof(ShootPayload));
      commandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle handle) {
      var commandBuffer = commandBufferSystem.CreateCommandBuffer();

      var destroyJob = new DestroyJob {
        CommandBuffer = commandBuffer,
      };

      var spawnJob = new SpawnJob {
        CommandBuffer = commandBuffer,
        Receivers = shootMessageQuery.ToComponentDataArray<Receiver>(Allocator.TempJob),
      };

      handle = spawnJob.Schedule(this, handle);
      handle = destroyJob.Schedule(this, handle);
      commandBufferSystem.AddJobHandleForProducer(handle);

      return handle;
    }

  }

}
