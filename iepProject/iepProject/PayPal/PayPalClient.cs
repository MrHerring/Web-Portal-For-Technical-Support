using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PayPalCheckoutSdk.Core;
using BraintreeHttp;

using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;

namespace iepProject.PayPal
{
    public class PayPalClient
    {
        /**
           Set up PayPal environment with sandbox credentials.
           In production, use LiveEnvironment.
        */
        public static PayPalEnvironment environment()
        {
            return new SandboxEnvironment("ASyeG4ju1BMd2GnKTsmmyX-oahCFHmOW-fMdHCFKrRx-e4y3oJMc_HBFnb25UOlOcxRAGBd7PUNTN9rm", "EKkFm__VdoKGll6vOmCxmMUVkyVGMO85lljOn1kIHIO96W0QKYa5Yy8aPJND4e1QvJBXohzsh94hX-dQ");
        }

        /**
            Returns PayPalHttpClient instance to invoke PayPal APIs.
         */
        public static HttpClient client()
        {
            return new PayPalHttpClient(environment());
        }

        public static HttpClient client(string refreshToken)
        {
            return new PayPalHttpClient(environment(), refreshToken);
        }

        /**
            Use this method to serialize Object to a JSON string.
        */
        public static String ObjectToJSONString(Object serializableObject)
        {
            MemoryStream memoryStream = new MemoryStream();
            var writer = JsonReaderWriterFactory.CreateJsonWriter(memoryStream, Encoding.UTF8, true, true, "  ");
            DataContractJsonSerializer ser = new DataContractJsonSerializer(serializableObject.GetType(), new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true });
            ser.WriteObject(writer, serializableObject);
            memoryStream.Position = 0;
            StreamReader sr = new StreamReader(memoryStream);
            return sr.ReadToEnd();
        }
    }
}