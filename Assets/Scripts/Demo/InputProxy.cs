using UnityEngine;
using Unity.Entities;
using System;
using Messenging;

namespace Messenging.Demo {

  [Serializable]
  public struct Input : IComponentData {}

  [RequiresEntityConversion]
  class InputProxy : ProxyBase<Input> {
    protected override Input Convert() {
      return new Input {};
    }
  }

}
