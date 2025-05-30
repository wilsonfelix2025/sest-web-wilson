using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SestWeb.Domain.Webhook
{
    public abstract class WebhookProperty
    {
        #region Properties

        [JsonProperty(PropertyName = "event_type")]
        public string EventType { get; set; }

        [JsonProperty(PropertyName = "event_data")]
        public object EventData { get; set; }

        [JsonProperty(PropertyName = "created_at")]
        public string CreateAt { get; set; }

        #endregion
    }


    public class AddWebhookProperty 
    {
        #region Properties

        [JsonProperty(PropertyName = "event_type")]
        public string EventType { get; set; }

        [JsonProperty(PropertyName = "event_data")]
        public AddEventData AddEventData { get; set; }

        [JsonProperty(PropertyName = "created_at")]
        public string CreateAt { get; set; }

        [JsonProperty(PropertyName = "add")]
        public List<string> Data { get; set; }
        #endregion
    }

    public class MoveWebhookProperty
    {
        #region Properties

        [JsonProperty(PropertyName = "event_type")]
        public string EventType { get; set; }

        [JsonProperty(PropertyName = "event_data")]
        public MoveEventData MoveEventData { get; set; }

        [JsonProperty(PropertyName = "created_at")]
        public string CreateAt { get; set; }

        [JsonProperty(PropertyName = "move")]
        public string[,] Data { get; set; } = new string[2, 3];

        #endregion
    }

    public class AddEventData
    {
        #region Properties

        [JsonProperty(PropertyName = "add")]
        public List<string> Data { get; set; }

        #endregion
    }

    public class MoveEventData
    {
        #region Properties

        [JsonProperty(PropertyName = "move")] 
        public string[,] Data { get; set; } = new string[2, 3];

        #endregion
    }






}
