using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using WpfCropper;

namespace UnitTestProject1 {
  [TestClass]
  public class UnitTest1 {
    [TestMethod]
    public void JsonConverter_TestMethod1() {

      var inputjson = "{'language':'tr','textAngle':0.0,'orientation':'Up','regions':[{'boundingBox':'149,56,383,237','lines':[{'boundingBox':'151,56,381,91','words':[{'boundingBox':'151,56,381,91','text':'UNİVERSAL'}]},{'boundingBox':'149,255,371,38','words':[{'boundingBox':'149,255,371,38','text':'UPS004-022224'}]}]}]}";
      var jsonconverter = new JsonConverter(inputjson);

      var list = jsonconverter.GetJsonValues();

      Assert.AreNotEqual(list.Count(),0);
    }
  }
}
