using UnityEngine;
using Unity.Entities;
using System;
using System.Runtime.Serialization;

namespace Messenging {

  [DataContract]
  public struct Address : IComponentData {
    [DataMember]
    public int Id;
  }

  public class AddressProxy : ProxyBase<Address> {
    public int Id;

    protected override Address Convert() {
      return new Address { Id = this.Id };
    }
  }

}
