using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FeesCalculator.BussinnesLogic.Messages
{
    [Serializable]
    public class OperationMessage
    {
        public OperationMessage(OperationMessageType type)
        {
            Type = type;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public  OperationMessageType Type { get; set; }

        [JsonProperty(Required = Required.Always)]
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }

        [DefaultValue(0.0)]
        public virtual decimal Rate { get; set; }

        public override string ToString()
        {
            return String.Format("{0} - {1}", Date, Amount);
        }

        [JsonProperty(Required = Required.Always)]
        [DefaultValue("")]
        public String RefDocument { get; set; }

        [DefaultValue(null)]
        public virtual String Comment { get; set; }
    }
}