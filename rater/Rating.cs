using Newtonsoft.Json;

public class Rating {
  [JsonIgnore]
  public string? Token { get; set; } = null;

  [JsonProperty("base")]
  public decimal Base => 1000m;

  [JsonProperty("action")]
  public decimal Action { get; set; } = 1m;

  [JsonProperty("survival")]
  public decimal Survival { get; set; } = 1m;

  [JsonProperty("exploration")]
  public decimal Exploration { get; set; } = 1m;

  [JsonProperty("creation")]
  public decimal Creation { get; set; } = 1m;

  [JsonProperty("total")]
  public decimal Total => Base * Action * Survival * Exploration * Creation;

  [JsonProperty("details")]
  public RatingData Details { get; set; } = new RatingData();
}
