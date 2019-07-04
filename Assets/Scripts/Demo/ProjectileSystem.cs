using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Messenging;

namespace Messenging.Demo {

  class ProjectileSystem : JobComponentSystem {

    [BurstCompile]
    struct MoveJob : IJobForEach<Translation, Rotation, Address, Projectile> {
      [ReadOnly]
      public float DeltaTime;

      public void Execute(
        ref Translation translation,
        ref Rotation rotation,
        [ReadOnly] ref Address address,
        [ReadOnly] ref Projectile projectile
      ) {
        var offset = new float3(0, 0, 0);
        switch (address.Id % 4) {
          case 0:
            offset.x = 10 * DeltaTime;
            break;
          case 1:
            offset.x = -10 * DeltaTime;
            break;
          case 2:
            offset.z = 10 * DeltaTime;
            break;
          case 3:
            offset.z = -10 * DeltaTime;
            break;
          default:
            break;
        }

        var rotationOffset = new float3(0, 0, 0);
        var rotationSpeed = 100 / (1 + (address.Id % 20));
        switch (address.Id % 6) {
          case 0:
            rotationOffset.y = DeltaTime * -0.5f * rotationSpeed;
            rotationOffset.z = DeltaTime * rotationSpeed;
            break;
          case 1:
            rotationOffset.x = DeltaTime * -0.5f * rotationSpeed;
            rotationOffset.z = -DeltaTime * rotationSpeed;
            break;
          case 2:
            rotationOffset.x = DeltaTime * rotationSpeed;
            rotationOffset.z = DeltaTime * 0.5f * rotationSpeed;
            break;
          case 3:
            rotationOffset.y = DeltaTime * rotationSpeed;
            rotationOffset.z = DeltaTime * -0.5f * rotationSpeed;
            break;
          case 4:
            rotationOffset.x = -DeltaTime * rotationSpeed;
            rotationOffset.z = DeltaTime * -0.5f * rotationSpeed;
            break;
          case 5:
            rotationOffset.y = -DeltaTime * rotationSpeed;
            rotationOffset.z = DeltaTime * -0.5f * rotationSpeed;
            break;
          default:
            break;
        }

        rotation.Value = math.mul(rotation.Value, quaternion.EulerXYZ(rotationOffset));
        translation.Value += math.rotate(rotation.Value, offset);
      }
    }

    protected override JobHandle OnUpdate(JobHandle handle) {
      var moveJob = new MoveJob {
        DeltaTime = Time.deltaTime,
      };

      return moveJob.Schedule(this, handle);
    }

  }

}
