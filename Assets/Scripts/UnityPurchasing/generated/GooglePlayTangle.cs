// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("0/Q0cwY7+Yvby+jCxCwGbC3iXFqxccxIMKqP6VSe/DU94+pqUNMUErjLmQIptjdJnyvUbjW1Q5gV50n2PX35khUKPtSanmYCWXEP+aEwZIHYW1VaathbUFjYW1ta2KxLh/G6wueEwD+7MdcSP25/sZ5MAhHWnXNWpesSp7Qg+TFrV6nbxZhygaoE8l9q2Ft4aldcU3DcEtytV1tbW19aWasiBR+BMI05wS244fASHxKsVQ6fvTXNqA4d5JHC1cj9JmaZYkNxQx0H+dVK0zDiaf90td9KRrZ2UIT6t7OicJcMo9CRm8LsoP4yuyU3vm8/99HQT5ot15o0w/7OJ990OzUUpplMQzxAd3gDGDB2KDEJn+oc5JpnaqbFxfNi1Mit+1hZW1pb");
        private static int[] order = new int[] { 10,11,4,4,11,5,8,10,9,10,10,12,13,13,14 };
        private static int key = 90;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
