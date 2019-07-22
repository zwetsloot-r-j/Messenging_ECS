using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Messenging;

namespace Messenging.Demo {

  class GrowSystem : MessageSystem<GrowPayload> {
    private static int lastId = 100001;
    private Unity.Mathematics.Random random;

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
        Receiver receiver;
        if (Message.FindReceiver(Receivers, address, out receiver)) {
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
        }
      }
    }

    protected override void Init() {
      random = new Unity.Mathematics.Random(1234);
    }

    protected override JobHandle HandleMessage(JobHandle handle, EntityCommandBuffer commandBuffer) {
      var spawnJob = new SpawnJob {
        CommandBuffer = commandBuffer,
        Receivers = Receivers,
        RandomNumber = random.NextFloat(4f) - 1f,
      };

      return spawnJob.Schedule(this, handle);
    }

  }

}
