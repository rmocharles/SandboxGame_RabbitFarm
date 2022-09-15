// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("IoTo41NdWp4siHm6ob9uZx0h0SfcP5Y8MOzPnvBypNsyd9bLQePMjMy/NIyIQimXRu75NZqjayTECI3auu3V8oRf3a5aw61V2X1dA1YGP09RZxfve/zqD6Q15aBFBNfirzWPuJCMmItleDY/8xgv6Lb19cUg+MrupYaZM+ADSFgoutp4yrST5w/Lx3TpLkJnE1kZM8BvLinbcJWQ+JlyZHUS4YLuHyyqESoXfXqi1bp38+chQP3KbhB0YwNXzstozH6pSwKBk3LvXd7979LZ1vVZl1ko0t7e3trf3NsujXkBhZUP6bIB/hjMmf7KEFcGE5YeIGjNZdj8sHbvCR+4M/IcfIJd3tDf713e1d1d3t7fdejBn4yVj9Qicm08lMTQiN3c3t/e");
        private static int[] order = new int[] { 9,2,13,11,11,6,12,7,12,12,12,11,12,13,14 };
        private static int key = 223;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
