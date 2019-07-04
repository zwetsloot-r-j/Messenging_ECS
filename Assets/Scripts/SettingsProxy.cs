using Unity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Messenging {

  public struct Settings : IComponentData {
    public bool ReplayEnabled;
    public bool LoggingEnabled;
    public int LogFilter;
  }

  public class SettingsProxy : ProxyBase<Settings> {
    public bool ReplayEnabled;
    public bool LoggingEnabled;
    public string[] LogFilter;

    protected override Settings Convert() {
      return new Settings {
        ReplayEnabled = this.ReplayEnabled,
        LoggingEnabled = this.LoggingEnabled,
        LogFilter = StringStore.Store(String.Join(":", this.LogFilter)),
      };
    }
  }

}
