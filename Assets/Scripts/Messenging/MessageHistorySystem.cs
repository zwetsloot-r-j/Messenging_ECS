using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using System.IO;
using System.Text;
using UnityEngine;

namespace Messenging {

  class MessageHistorySystem : ComponentSystem {

    public static int Frame = 0;

    private StreamWriter writer;
    private StreamReader reader;

    private Message next;

    protected override void OnUpdate() {
      if (!GlobalSettings.Initialized) {
        return;
      }

      Frame++;

      if (GlobalSettings.ReplayEnabled) {
        Replay();
        return;
      }

      Record();
    }

    protected override void OnDestroy() {
      if (reader != null) {
        reader.Dispose();
      }

      if (writer != null) {
        writer.Dispose();
      }
    }

    private void Replay() {
      if (writer != null) {
        writer.Dispose();
      }

      if (reader == null) {
        reader = new StreamReader(Path.Combine(Application.persistentDataPath, "MessageHistory.txt"));
      }

      while (ReplayNext()) {};
    }

    private bool ReplayNext() {
      if (next != null && next.Frame <= Frame) {
        next.Send(PostUpdateCommands);
        next = null;
        return true;
      }

      if (next != null && next.Frame > Frame) {
        return false;
      }

      var line = reader.ReadLine();
      if (line == null) {
        return false;
      }

      next = Message.Deserialize(line);
      return true;
    }

    private void Record() {
      if (writer == null) {
        writer = new StreamWriter(Path.Combine(Application.persistentDataPath, "MessageHistory.txt"));
      }

      Entities.ForEach((Entity entity, ref MessageHistoryRecord messageHistoryRecord) => {
        var record = StringStore.Retrieve(messageHistoryRecord.Record);
        writer.WriteLine(record);
        PostUpdateCommands.DestroyEntity(entity);
      });
    }

  }

}
