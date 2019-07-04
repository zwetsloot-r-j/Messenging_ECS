using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Runtime.Serialization;
using Monads.Result;
using Newtonsoft.Json;

namespace Messenging {

  [DataContract]
  public class Message {
    [DataMember]
    public int To;
    [DataMember]
    public int From;
    [DataMember]
    public int Frame;

    public IPayload Payload;
    public bool IsReplayTarget = false;

    [DataMember]
    protected IPayloadConverter converter = new DefaultPayloadConverter();

    public static IList<IMessageMiddleWare> MiddleWare = new List<IMessageMiddleWare>() {
      new MessageHistoryMiddleWare(),
      new LoggerMiddleWare(),
    };

    private static readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings {
      TypeNameHandling = TypeNameHandling.All
    };

    public virtual void Send(EntityCommandBuffer commandBuffer) {
      DoSend<EmptyPayload>(commandBuffer);
    }

    protected void DoSend<T>(EntityCommandBuffer commandBuffer) where T : struct, IPayload {
      ApplyMiddleWare(commandBuffer).Do(msg => CreateEntity<T>(msg, commandBuffer));
    }

    void CreateEntity<T>(Message message, EntityCommandBuffer commandBuffer) where T : struct, IPayload {
      var entity = commandBuffer.CreateEntity();

      commandBuffer.AddComponent(entity, new Receiver { Id = message.To });
      commandBuffer.AddComponent(entity, (T)Payload);
      if (message.From != null) {
        commandBuffer.AddComponent(entity, new Sender { Id = message.From });
      }
    }

    IResult<Message, Reason> ApplyMiddleWare(EntityCommandBuffer commandBuffer) {
      return MiddleWare.Aggregate<IMessageMiddleWare, Func<Message, IResult<Message, Reason>>>(
        (msg) => new Ok<Message, Reason>(msg),
        (acc, next) => next.Apply(acc, commandBuffer)
      )(this);
    }

    public string Serialize() {
      converter.Convert(Payload);
      return JsonConvert.SerializeObject(this, jsonSerializerSettings);
    }

    public static Message Deserialize(string serialized) {
      var message = JsonConvert.DeserializeObject<Message>(serialized, jsonSerializerSettings);
      message.DeserializePayload();
      message.IsReplayTarget = true;
      return message;
    }

    public void DeserializePayload() {
      Payload = converter.Revert();
    }
  }
}
