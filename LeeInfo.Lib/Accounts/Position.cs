using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Collections.Generic;

namespace Connect_API.Accounts
{
    public class Position
    {
        [JsonProperty("positionID")]
        public long PositionID { get; set; }
        [JsonProperty("symbolId")]
        public int SymbolId { get; set; }
        [JsonProperty("label")]
        public string Label { get; set; }
        [JsonProperty("comment")]
        public string Comment { get; set; }
        [JsonProperty("channel")]
        public string Channel { get; set; }
        [JsonProperty("symbolName")]
        public string SymbolName { get; set; }
        [JsonProperty("tradeSide")]
        public string TradeSide { get; set; }
        [JsonProperty("volume")]
        public long Volume { get; set; }
        [JsonProperty("lot")]
        public double? Lot { get; set; }
        [JsonProperty("entryTimestamp")]
        public string EntryTimestamp { get; set; }
        [JsonProperty("utcLastUpdateTimestamp")]
        public string UtcLastUpdateTimestamp { get; set; }
        [JsonProperty("entryPrice")]
        public double EntryPrice { get; set; }
        [JsonProperty("currentPrice")]
        public double? CurrentPrice { get; set; }
        [JsonProperty("takeProfit")]
        public double? TakeProfit { get; set; }
        [JsonProperty("stopLoss")]
        public double? StopLoss { get; set; }
        [JsonProperty("marginRate")]
        public double MarginRate { get; set; }
        [JsonProperty("commission")]
        public double Commission { get; set; }
        [JsonProperty("swap")]
        public double Swap { get; set; }
        [JsonProperty("profitInPips")]
        public double? ProfitInPips { get; set; }
        [JsonProperty("profit")]
        public double? Profit { get; set; }

        public static List<Position> GetPositions(string apiUrl, string accountID, string accessToken)
        {
            var client = new RestClient(apiUrl);
            var request = new RestRequest(@"connect/tradingaccounts/" + accountID + "/positions?oauth_token=" + accessToken);
            var responsePosition = client.Execute<Position>(request);
            return JsonConvert.DeserializeObject<List<Position>>((JObject.Parse(responsePosition.Content)["data"]).ToString());
        }
    }
}