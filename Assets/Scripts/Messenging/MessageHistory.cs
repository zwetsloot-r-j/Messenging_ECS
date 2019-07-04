using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Concurrent;

namespace Messenging {

  public class MessageHistory : MonoBehaviour {
    public bool Replay = false;

    static MessageHistory instance;
    public static MessageHistory Instance {
      get {
        if (instance != null) {
          return instance;
        }

        instance = FindObjectOfType<MessageHistory>();

        if (instance != null) {
          return instance;
        }

        var obj = new GameObject("MessageHistory");
        instance = obj.AddComponent<MessageHistory>();

        return instance;
      }
    }

    static ConcurrentQueue<string> messages;

    StreamWriter writer;
    StreamReader reader;

    public static void RecordMessage(Message message) {
      messages.Enqueue(message.Serialize());
    }
  }

}
