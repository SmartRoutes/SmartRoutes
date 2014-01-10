using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartRoutes.Model.Gtfs;

namespace SmartRoutes.Model.Test
{
    [TestClass]
    public class ExtensionMethodsTest
    {
        [TestMethod]
        public void GetL1DistanceInFeet_OrderDistances()
        {
            // ARRANGE, ACT
            double shortest = GetL1DistanceInFeet(0.0, 0.0, 0.01, 0.01);
            double middle = GetL1DistanceInFeet(0.0, 0.0, 0.02, 0.02);
            double longest = GetL1DistanceInFeet(0.0, 0.0, 0.04, 0.04);

            // ASSERT
            Assert.IsTrue(shortest < middle);
            Assert.IsTrue(middle < longest);
        }

        [TestMethod]
        public void GetL1DistanceInFeet_ZeroDistance()
        {
            // ARRANGE, ACT
            double distance = GetL1DistanceInFeet(0.0, 0.0, 0.0, 0.0);

            // ASSERT
            Assert.AreEqual(0.0, distance);
        }

        [TestMethod]
        public void GetL1DistanceInFeet_Commutative()
        {
            // ARRANGE, ACT
            double distanceA = GetL1DistanceInFeet(0.0, 0.0, 0.01, 0.01);
            double distanceB = GetL1DistanceInFeet(0.01, 0.01, 0.0, 0.0);

            // ASSERT
            Assert.AreEqual(distanceA, distanceB);
        }

        private static double GetL1DistanceInFeet(double latitudeA, double longitudeA, double latitudeB, double longitudeB)
        {
            // create some ILocation objects
            var locationA = new Stop {Latitude = latitudeA, Longitude = longitudeA};
            var locationB = new Stop {Latitude = latitudeB, Longitude = longitudeB};

            // calculate the distance
            return locationA.GetL1DistanceInFeet(locationB);
        }
    }
}