using System;
using System.Runtime.Serialization;

namespace Contoso.Infrastructure.Context
{
    [DataContract]
    [Serializable]
    public class MyAppContext
    {
        // [DataMember] public string ApplicationId { get; internal set; }
        [DataMember]
        public string ApplicationId { get; set; }

        [DataMember]
        public string UserCode { get; set; }

        [DataMember]
        public string OrchestrationId { get; set; }

        public override string ToString() => string.Format("AppId: {ApplicationId}, UserCode: {UserCode}, OrchestrationId: {OrchestrationId}", ApplicationId, UserCode, OrchestrationId);
    }
}
