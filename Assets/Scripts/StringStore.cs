using System.Collections.Generic;

namespace Messenging {

  public static class StringStore {

    private static int lastId = 0;
    private static Dictionary<int, string> store = new Dictionary<int, string>();

    public static int Store(string content) {
      var id = lastId++;
      store.Add(id, content);

      return id;
    }

    public static string Retrieve(int id) {
      var content = store[id];
      store.Remove(id);

      return content;
    }

  }

}
