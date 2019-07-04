using UnityEngine;
using Unity.Entities;
using System;
using System.Runtime.Serialization;

namespace Messenging {

  [DataContract]
  public struct Sender : IComponentData {
    [DataMember]
    public int Id;
  }

  public class SenderProxy : ProxyBase<Sender> {
    public int Id;

    protected override Sender Convert() {
      return new Sender { Id = this.Id };
    }
  }

}
