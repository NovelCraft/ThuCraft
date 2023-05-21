using Newtonsoft.Json;

public struct Rating {
  [JsonProperty("base")]
  public decimal Base => 1000m;

  [JsonProperty("action")]
  public decimal Action { get; set; }

  [JsonProperty("survival")]
  public decimal Survival { get; set; }

  [JsonProperty("exploration")]
  public decimal Exploration { get; set; }

  [JsonProperty("creation")]
  public decimal Creation { get; set; }

  [JsonProperty("total")]
  public decimal Total => Base * Action * Survival * Exploration * Creation;
}
