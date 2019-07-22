using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Messenging;

namespace Messenging.Demo {

  class ShootSystem : MessageSystem<ShootPayload> {
    private static int lastId = 101;

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
        Receiver receiver;
        if (Message.FindReceiver(Receivers, address, out receiver)) {
          for (var x = 0; x < prototype.SpawnCount; x++) {
            var projectile = CommandBuffer.Instantiate(prototype.Prototype);
            var position = math.transform(localToWorld.Value, new float3(0, 0, 0));
            CommandBuffer.SetComponent(projectile, new Translation { Value = position });
            CommandBuffer.SetComponent(projectile, new Address { Id = lastId });
            lastId++;
          }
        }
      }
    }

    protected override JobHandle HandleMessage(JobHandle handle, EntityCommandBuffer commandBuffer) {
      var spawnJob = new SpawnJob {
        CommandBuffer = commandBuffer,
        Receivers = Receivers,
      };

      return spawnJob.Schedule(this, handle);
    }

  }

}
