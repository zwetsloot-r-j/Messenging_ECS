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

    public virtual void Send(EntityManager entityManager) {
      DoSend<EmptyPayload>(entityManager);
    }

    protected void DoSend<T>(EntityCommandBuffer commandBuffer) where T : struct, IPayload {
      ApplyMiddleWare(commandBuffer).Do(msg => CreateEntity<T>(msg, commandBuffer));
    }

    protected void DoSend<T>(EntityManager entityManager) where T : struct, IPayload {
      ApplyMiddleWare(entityManager).Do(msg => CreateEntity<T>(msg, entityManager));
    }

    void CreateEntity<T>(Message message, EntityCommandBuffer commandBuffer) where T : struct, IPayload {
      var entity = commandBuffer.CreateEntity();

      commandBuffer.AddComponent(entity, new Receiver { Id = message.To });
      commandBuffer.AddComponent(entity, (T)Payload);
      if (message.From != null) {
        commandBuffer.AddComponent(entity, new Sender { Id = message.From });
      }
    }

    void CreateEntity<T>(Message message, EntityManager entityManager) where T : struct, IPayload {
      var entity = entityManager.CreateEntity();

      entityManager.AddComponentData(entity, new Receiver { Id = message.To });
      entityManager.AddComponentData(entity, (T)Payload);
      if (message.From != null) {
        entityManager.AddComponentData(entity, new Sender { Id = message.From });
      }
    }

    IResult<Message, Reason> ApplyMiddleWare(EntityCommandBuffer commandBuffer) {
      return MiddleWare.Aggregate<IMessageMiddleWare, Func<Message, IResult<Message, Reason>>>(
        (msg) => new Ok<Message, Reason>(msg),
        (acc, next) => next.Apply(acc, commandBuffer)
      )(this);
    }

    IResult<Message, Reason> ApplyMiddleWare(EntityManager entityManager) {
      return MiddleWare.Aggregate<IMessageMiddleWare, Func<Message, IResult<Message, Reason>>>(
        (msg) => new Ok<Message, Reason>(msg),
        (acc, next) => next.Apply(acc, entityManager)
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

    public static bool FindPayloadForReceiver<TPayload>(
      NativeArray<Receiver> receivers,
      NativeArray<TPayload> payloads,
      Address address,
      out TPayload payload,
      out Receiver receiver
    ) where TPayload : struct, IPayload {
      for (var i = 0; i < receivers.Length; i++) {
        if (receivers[i].Id == address.Id) {
          receiver = receivers[i];
          payload = payloads[i];
          return true;
        }
      }
      receiver = default(Receiver);
      payload = default(TPayload);
      return false;
    }


    public static bool FindReceiver(
      NativeArray<Receiver> receivers,
      Address address,
      out Receiver receiver
    ) {
      for (var i = 0; i < receivers.Length; i++) {
        if (receivers[i].Id == address.Id) {
          receiver = receivers[i];
          return true;
        }
      }
      receiver = default(Receiver);
      return false;
    }
  }
}
