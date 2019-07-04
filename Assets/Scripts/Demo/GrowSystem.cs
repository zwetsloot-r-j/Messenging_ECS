using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Messenging;

namespace Messenging.Demo {

  class GrowSystem : JobComponentSystem {
    private static int lastId = 100001;
    private Unity.Mathematics.Random random;

    BeginInitializationEntityCommandBufferSystem commandBufferSystem;
    EntityQuery growMessageQuery;

    //[BurstCompile]
    struct SpawnJob : IJobForEach<Address, PlantPrototype, LocalToWorld> {
      [ReadOnly]
      public EntityCommandBuffer CommandBuffer;

      [ReadOnly]
      [DeallocateOnJobCompletion]
      public NativeArray<Receiver> Receivers;

      [ReadOnly]
      public float RandomNumber;

      public void Execute(
        [ReadOnly] ref Address address,
        [ReadOnly] ref PlantPrototype prototype,
        [ReadOnly] ref LocalToWorld localToWorld
      ) {
        for (var i = 0; i < Receivers.Length; i++) {
          if (Receivers[i].Id == address.Id) {
            for (var x = 0; x < prototype.SpawnCount; x++) {
              var plant = CommandBuffer.Instantiate(prototype.Prototype);
              var position = math.transform(localToWorld.Value, new float3(
                RandomNumber,
                0,
                RandomNumber * -1f
              ));
              CommandBuffer.SetComponent(plant, new Translation { Value = position });
              CommandBuffer.SetComponent(plant, new Address { Id = lastId });
              lastId++;
            }
            break;
          }
        }
      }
    }

    [BurstCompile]
    struct DestroyJob : IJobForEachWithEntity<GrowPayload, Receiver> {
      [ReadOnly]
      public EntityCommandBuffer CommandBuffer;

      public void Execute(Entity entity, int i, [ReadOnly] ref GrowPayload payload, [ReadOnly] ref Receiver receiver) {
        CommandBuffer.DestroyEntity(entity);
      }
    }

    protected override void OnCreate() {
      growMessageQuery = GetEntityQuery(typeof(Receiver), typeof(GrowPayload));
      commandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
      random = new Unity.Mathematics.Random(1234);
    }

    protected override JobHandle OnUpdate(JobHandle handle) {
      var commandBuffer = commandBufferSystem.CreateCommandBuffer();

      var destroyJob = new DestroyJob {
        CommandBuffer = commandBuffer,
      };

      var spawnJob = new SpawnJob {
        CommandBuffer = commandBuffer,
        Receivers = growMessageQuery.ToComponentDataArray<Receiver>(Allocator.TempJob),
        RandomNumber = random.NextFloat(4f) - 1f,
      };

      handle = spawnJob.Schedule(this, handle);
      handle = destroyJob.Schedule(this, handle);
      commandBufferSystem.AddJobHandleForProducer(handle);

      return handle;
    }

  }

}
