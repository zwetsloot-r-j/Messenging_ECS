using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;

namespace Messenging {

  class SettingsSystem : JobComponentSystem {
    BeginInitializationEntityCommandBufferSystem commandBufferSystem;

    // [BurstCompile] MEMO: writing to static is not supported by burst
    struct InitJob : IJobForEachWithEntity<Settings> {
      [ReadOnly]
      public EntityCommandBuffer CommandBuffer;

      public void Execute(Entity entity, int index, [ReadOnly] ref Settings settings) {
        GlobalSettings.Initialized = true;
        GlobalSettings.ReplayEnabled = settings.ReplayEnabled;
        GlobalSettings.LoggingEnabled = settings.LoggingEnabled;
        GlobalSettings.LogFilter = StringStore.Retrieve(settings.LogFilter).Split(':');
        CommandBuffer.DestroyEntity(entity);
      }
    }

    protected override void OnCreate() {
      commandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle handle) {
      var initJob = new InitJob {
        CommandBuffer = commandBufferSystem.CreateCommandBuffer(),
      };

      handle = initJob.Schedule(this, handle);
      commandBufferSystem.AddJobHandleForProducer(handle);

      return handle;
    }
  }

}
