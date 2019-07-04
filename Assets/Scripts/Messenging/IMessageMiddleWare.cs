using System;
using Monads.Result;
using Unity.Entities;

namespace Messenging {

  public interface IMessageMiddleWare {
    Func<Message, IResult<Message, Reason>> Apply(
      Func<Message, IResult<Message, Reason>> next,
      EntityCommandBuffer commandBuffer
    );
  }

}
