using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Contoso.Infrastructure.Context
{

    [DataContract]
    public class Header<T> where T : class
    {
        public static string TypeName { get; private set; }
        public static string TypeNamespace { get; private set; }

        [DataMember]
        public T Value
        { get; private set; }

        static Header()
        {
            // Verify [DataContract] or [Serializable] on T
            // Debug.Assert(IsDataContract(typeof(T)));
            TypeNamespace = "net.clr:" + typeof(T).FullName;
            TypeName = "Header";
        }

        static bool IsDataContract(Type type)
        {
            object[] attributes = type.GetCustomAttributes(typeof(DataContractAttribute), false);
            return attributes.Length == 1;
        }

        public Header() : this(default(T))
        {

        }

        public Header(T value)
        {
            Value = value;
        }


        private static string Key => typeof(T).Name;

        public static void Set(IDictionary<string, object> dict, T value)
        {
            Debug.Assert(dict != null);
            Debug.Assert(value != default(T));

            dict[Key] = value;
        }

        public static T Get(IDictionary<string, object> dict)
        {
            Debug.Assert(dict != null);
            
            var header = dict.TryGetValue(Key, out var val) ? val as T : null;
            return header;
        }
    }
}
