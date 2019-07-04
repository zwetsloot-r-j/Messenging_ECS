using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Messenging;

namespace Messenging.Demo {

  class PlantSystem : JobComponentSystem {

    [BurstCompile]
    struct GrowJob : IJobForEach<NonUniformScale, Plant, Lifetime> {
      public void Execute(ref NonUniformScale scale, [ReadOnly] ref Plant plant, [ReadOnly] ref Lifetime lifetime) {
        scale.Value.y = math.min((float)lifetime.CurrentLifetime * 100f, 100f);
      }
    }

    protected override JobHandle OnUpdate(JobHandle handle) {
      var growJob = new GrowJob();
      handle = growJob.Schedule(this, handle);

      return handle;
    }

  }

}
