using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace WpfCropper {
  public class JsonConverter {
    private string jsonString;
    public JsonConverter(string jsonstring) {
      this.jsonString = jsonstring;
    }

    public IEnumerable<string> GetJsonValues() {
      JObject rss = JObject.Parse(jsonString);
      List<string> list = new List<string>();

      JArray results = (JArray)rss["regions"][0]["lines"];

      foreach (var word in results) {
        list.Add(word["words"][0]["text"].Value<string>());
      }

      return list;
    }
  }
}
