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
      //var inputjson = "{'language':'en','textAngle':0.0,'orientation':'Up','regions':[{'boundingBox':'7,9,411,126','lines':[{'boundingBox':'7,9,410,20','words':[{'boundingBox':'7,9,142,20','text':'TRAVEL'},{'boundingBox':'180,11,69,18','text':'THE'},{'boundingBox':'281,11,136,18','text':'WORLD'}]},{'boundingBox':'45,45,347,20','words':[{'boundingBox':'45,45,59,19','text':'SEE'},{'boundingBox':'136,45,68,19','text':'THE'},{'boundingBox':'238,45,154,20','text':'SIGHTS,'}]},{'boundingBox':'31,81,361,18','words':[{'boundingBox':'31,81,91,18','text':'HAVE'},{'boundingBox':'156,81,115,18','text':'GREAT'},{'boundingBox':'305,81,87,18','text':'DAYS'}]},{'boundingBox':'10,117,408,18','words':[{'boundingBox':'10,117,73,18','text':'AND'},{'boundingBox':'117,117,131,18','text':'BETTER'},{'boundingBox':'280,117,138,18','text':'NIGHTS'}]}]}]}";
      var jsonconverter = new JsonConverter(inputjson);

      var list = jsonconverter.GetJsonValues();

      Assert.AreNotEqual(list.Count(),0);
    }
  }
}
