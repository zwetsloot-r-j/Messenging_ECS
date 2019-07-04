using System;
using Monads.Result;
using Unity.Entities;

namespace Messenging {

  public class MessageHistoryMiddleWare : IMessageMiddleWare {
    public Func<Message, IResult<Message, Reason>> Apply(
      Func<Message, IResult<Message, Reason>> next,
      EntityCommandBuffer commandBuffer
    ) {
      return (message) => {
        if (GlobalSettings.ReplayEnabled && !message.IsReplayTarget) {
          return new Error<Message, Reason>(Reason.Canceled);
        }

        if (GlobalSettings.ReplayEnabled && message.IsReplayTarget) {
          return next(message);
        }

        message.Frame = MessageHistorySystem.Frame;
        var entity = commandBuffer.CreateEntity();
        commandBuffer.AddComponent(entity, new MessageHistoryRecord {
          Record = StringStore.Store(message.Serialize()),
        });

        return next(message);
      };
    }
  }

}
