using System.Linq;
using System.Threading;
using Nimbus.Internal;
using Nimbus.Internal.Network;
using Nimbus.Runtime.Scripts;
using Nimbus.ScriptableObjects;
using NUnit.Framework;
using OpenRTB.Request;
using UnityEngine;
using Gender = UnityEngine.Analytics.Gender;

namespace Nimbus.Tests
{
    public class RTBDataTest
    {
        [Test]
        public void TestUserAge()
        {
            string objectName = "MyGameObject";
            GameObject gameObject = new GameObject(objectName);
            var manager = gameObject.AddComponent<NimbusManager>();
            manager.SetUserAge(25);
            BidRequest putBidRequest = new BidRequest
            {
                Imp = new[]
                {
                    new Imp
                    {
                        Ext = new ImpExt()
                        {
                            Position = "test",
                        }
                    }
                }
            };
            var gotBidRequest = manager.ApplyUserData(putBidRequest);
            Assert.AreEqual("25", gotBidRequest.User.Data.First().Segment.First().Value);
        }
        
        [Test]
        public void TestUserGender()
        {
            string objectName = "MyGameObject";
            GameObject gameObject = new GameObject(objectName);
            var manager = gameObject.AddComponent<NimbusManager>();
            manager.SetUserGender(OpenRTB.Request.Gender.O);
            BidRequest putBidRequest = new BidRequest
            {
                Imp = new[]
                {
                    new Imp
                    {
                        Ext = new ImpExt()
                        {
                            Position = "test",
                        }
                    }
                }
            };
            var gotBidRequest = manager.ApplyUserData(putBidRequest);
            Assert.AreEqual("O", gotBidRequest.User.Data.First().Segment.First().Value);
        }
        
        [Test]
        public void TestUserData()
        {
            string objectName = "MyGameObject";
            GameObject gameObject = new GameObject(objectName);
            var manager = gameObject.AddComponent<NimbusManager>();
            manager.SetUserGender(OpenRTB.Request.Gender.F);
            manager.SetUserAge(27);
            BidRequest putBidRequest = new BidRequest
            {
                Imp = new[]
                {
                    new Imp
                    {
                        Ext = new ImpExt()
                        {
                            Position = "test",
                        }
                    }
                }
            };
            var gotBidRequest = manager.ApplyUserData(putBidRequest);
            var gotGender = gotBidRequest.User.Data.First().Segment.FirstOrDefault(seg => seg.Name == "gender").Value;
            var gotAge = gotBidRequest.User.Data.First().Segment.FirstOrDefault(seg => seg.Name == "age").Value;
            Assert.AreEqual("F", gotGender);
            Assert.AreEqual("27", gotAge);
        }
    }
}