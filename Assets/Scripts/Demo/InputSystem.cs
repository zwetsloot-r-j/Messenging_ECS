using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using InputData = Messenging.Demo.Input;
using Messenging;

namespace Messenging.Demo {

  public class InputSystem : JobComponentSystem {
    private enum ButtonState {
      Pressed,
      Released
    }

    BeginInitializationEntityCommandBufferSystem commandBufferSystem;

    //[BurstCompile] // MEMO: cannot create entities with burst compile on yet
    struct Job : IJobForEach<Address, InputData> {
      [ReadOnly]
      public ButtonState Forward; 

      [ReadOnly]
      public ButtonState Backward;

      [ReadOnly]
      public ButtonState Left;

      [ReadOnly]
      public ButtonState Right;

      [ReadOnly]
      public ButtonState Fire;

      [ReadOnly]
      public ButtonState Special;

      [ReadOnly]
      public float DeltaTime;

      [ReadOnly]
      public EntityCommandBuffer CommandBuffer;

      public void Execute([ReadOnly] ref Address address, [ReadOnly] ref InputData input) {
        var id = address.Id;
        var translation = new float3(0, 0, 0);
        var movementSpeed = 10;

        if (Forward == ButtonState.Pressed) {
          translation.z += DeltaTime * movementSpeed;
        }

        if (Backward == ButtonState.Pressed) {
          translation.z -= DeltaTime * movementSpeed;
        }

        if (Left == ButtonState.Pressed) {
          translation.x += DeltaTime * movementSpeed;
        }

        if (Right == ButtonState.Pressed) {
          translation.x -= DeltaTime * movementSpeed;
        }

        if (translation.x != 0 || translation.z != 0) {
          var message = new MoveMessage {
            To = id,
            From = id,
            Payload = new MovePayload {
              Translation = translation,
            },
          };

          message.Send(CommandBuffer);
        }

        if (Fire == ButtonState.Pressed) {
          var shootMessage = new ShootMessage {
            To = id,
            From = id,
            Payload = new ShootPayload(),
          };

          shootMessage.Send(CommandBuffer);
        }

        if (Special == ButtonState.Pressed) {
          var growMessage = new GrowMessage {
            To = id,
            From = id,
            Payload = new GrowPayload(),
          };

          growMessage.Send(CommandBuffer);
        }
      }
    }

    protected override void OnCreate() {
      commandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle handle) {
      var job = new Job {
        Forward = UnityEngine.Input.GetKey("w") ? ButtonState.Pressed : ButtonState.Released,
        Backward = UnityEngine.Input.GetKey("s") ? ButtonState.Pressed : ButtonState.Released,
        Left = UnityEngine.Input.GetKey("a") ? ButtonState.Pressed : ButtonState.Released,
        Right = UnityEngine.Input.GetKey("d") ? ButtonState.Pressed : ButtonState.Released,
        Fire = UnityEngine.Input.GetKey(KeyCode.Return) ? ButtonState.Pressed : ButtonState.Released,
        Special = UnityEngine.Input.GetKey(KeyCode.Space) ? ButtonState.Pressed : ButtonState.Released,
        DeltaTime = Time.deltaTime,
        CommandBuffer = commandBufferSystem.CreateCommandBuffer()
      };

      handle = job.Schedule(this, handle);
      commandBufferSystem.AddJobHandleForProducer(handle);

      return handle;
    }
  }

}
