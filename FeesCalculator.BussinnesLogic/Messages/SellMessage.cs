using System;
using System.Runtime.Serialization;
using FeesCalculator.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FeesCalculator.BussinnesLogic.Messages
{
    [Serializable]
    [KnownType(typeof(OperationMessage))]
    public class SellMessage: OperationMessage
    {
        public SellMessage()
            : base(OperationMessageType.Sell)
        {
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public SellType SellType { get; set; }

        public override String Comment
        {
            get
            {
                return String.Format("ƒанна€ операци€ содержить информацию об {0} продаже валюты. {1}",
                    SellType == SellType.Mandatory ? "об€зательной (30%)" : "свободной", 
                    SellType == SellType.Mandatory ? " урс будет вз€т по нац. банку на день продажи." : " урс продажи должне быть указан в текущей операции.");
            }
        }
    }
}