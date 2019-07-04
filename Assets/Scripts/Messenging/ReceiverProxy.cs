using UnityEngine;
using Unity.Entities;
using System;
using System.Runtime.Serialization;

namespace Messenging {

  [DataContract]
  public struct Receiver : IComponentData {
    [DataMember]
    public int Id;
  }

  public class ReceiverProxy : ProxyBase<Receiver> {
    public int Id;

    protected override Receiver Convert() {
      return new Receiver { Id = this.Id };
    }
  }

}
