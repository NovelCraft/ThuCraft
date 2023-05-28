using System.Collections.Generic;
using Newtonsoft.Json;

public class RatingData {
  [JsonIgnore]
  public string? Token { get; set; } = null;

  [JsonProperty("name")]
  public string? Name { get; set; } = null;

  // Action
  [JsonProperty("actions_of_perform")]
  public int ActionsOfPerform { get; set; } = 0;

  [JsonProperty("actions_of_get")]
  public int ActionsOfGet { get; set; } = 0;

  [JsonProperty("actions_of_ping")]
  public int ActionsOfPing { get; set; } = 0;

  // Survival
  [JsonProperty("kills")]
  public int Kills { get; set; } = 0;

  [JsonProperty("deaths")]
  public int Deaths { get; set; } = 0;

  [JsonProperty("damage_dealt")]
  public decimal DamageDealt { get; set; } = 0m;

  [JsonProperty("damage_taken")]
  public decimal DamageTaken { get; set; } = 0m;

  [JsonProperty("healed")]
  public decimal Healed { get; set; } = 0m;

  // Exploration
  [JsonIgnore]
  public HashSet<(int, int, int)> SectionVisisted { get; set; } = new HashSet<(int, int, int)>();

  [JsonProperty("section_visisted")]
  public int SectionVisistedCount => SectionVisisted.Count;

  [JsonProperty("coal_ore_mined")]
  public int CoalOreMined { get; set; } = 0;

  [JsonProperty("iron_ore_mined")]
  public int IronOreMined { get; set; } = 0;

  [JsonProperty("gold_ore_mined")]
  public int GoldOreMined { get; set; } = 0;

  [JsonProperty("diamond_ore_mined")]
  public int DiamondOreMined { get; set; } = 0;

  [JsonProperty("redstone_ore_mined")]
  public int LeavesBroken { get; set; } = 0;

  [JsonIgnore]
  public HashSet<int> BlockTypesBroken { get; set; } = new HashSet<int>();

  [JsonProperty("block_types_broken")]
  public int BlockTypesBrokenCount => BlockTypesBroken.Count;

  // Creation
  [JsonIgnore]
  public HashSet<int> ItemTypesGot { get; set; } = new HashSet<int>();

  [JsonProperty("item_types_got")]
  public int ItemTypesGotCount => ItemTypesGot.Count;

  [JsonIgnore]
  public HashSet<int> BlockTypesPlaced { get; set; } = new HashSet<int>();

  [JsonProperty("block_types_placed")]
  public int BlockTypesPlacedCount => BlockTypesPlaced.Count;
}
