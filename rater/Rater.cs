using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

/// <summary>
/// Rater rates records.
/// </summary>
public class Rater {
  public class RatingData {
    public int Kills { get; set; } = 0;
    public int Deaths { get; set; } = 0;
    public decimal DamageDealt { get; set; } = 0m;
    public decimal DamageTaken { get; set; } = 0m;
    public decimal Healed { get; set; } = 0m;
    public HashSet<(int, int, int)> SectionVisisted { get; set; } = new HashSet<(int, int, int)>();
  }

  private Dictionary<int, RatingData> _ratingData = new Dictionary<int, RatingData>(); // uid -> rating data

  /// <summary>
  /// Creates a new instance of Rater.
  /// </summary>
  public Rater() {
    // Nothing to do here.
  }

  /// <summary>
  /// Adds a record.
  /// </summary>
  /// <param name="record">The record to add.</param>
  public void AddRecord(JObject record) {
    try {
      string identifier = record["identifier"]?.ToString() ?? throw new Exception("Missing record identifier.");
      JObject recordData = record["data"]?.ToObject<JObject>() ?? throw new Exception("Missing record data.");

      switch (identifier) {
        case "after_block_change":
          HandleAfterBlockChange(recordData);
          break;

        case "after_entity_despawn":
          HandleAfterEntityDespawn(recordData);
          break;

        case "after_entity_hurt":
          HandleAfterEntityHurt(recordData);
          break;

        default:
          break;
      }

    } catch (Exception e) {
      throw new Exception($"Bad record: {e.Message}", e);
    }
  }

  /// <summary>
  /// Gets the rating.
  /// </summary>
  /// <param name="uniqueId">The unique ID.</param>
  /// <returns>The rating.</returns>
  public Rating GetRating(int uniqueId) {
    // TODO: Implement.
    RatingData ratingData = GetRatingData(uniqueId);
    return new Rating {
      Action = 1,
      Survival = (1 + 0.001m * ratingData.DamageDealt + 0.001m * ratingData.DamageTaken)
               / (1 + 0.2m * ratingData.Deaths),
      Exploration = 1,
      Creation = 1
    };
  }

  /// <summary>
  /// Gets the unique ID list.
  /// </summary>
  /// <returns>The unique ID list.</returns>
  public List<int> GetUniqueIdList() {
    return new List<int>(_ratingData.Keys);
  }

  private RatingData GetRatingData(int uniqueId) {
    if (!_ratingData.ContainsKey(uniqueId)) {
      _ratingData.Add(uniqueId, new RatingData {
        Kills = 0,
        Deaths = 0,
        DamageDealt = 0m,
        DamageTaken = 0m,
        Healed = 0m
      });
    }

    return _ratingData[uniqueId];
  }

  private void HandleAfterBlockChange(JObject recordData) {
    // TODO: Implement.
  }

  private void HandleAfterEntityDespawn(JObject recordData) {
    JArray DespawnList = recordData["despawn_list"] as JArray ?? throw new Exception("Missing despawn list.");

    foreach (JObject? despawn in DespawnList) {
      if (despawn is null) {
        throw new Exception("Null despawn item.");
      }

      int uniqueId = (int?)despawn["unique_id"] ?? throw new Exception("Missing unique ID.");
      GetRatingData(uniqueId).Deaths++;
    }
  }

  private void HandleAfterEntityHurt(JObject recordData) {
    JArray hurtList = recordData["hurt_list"] as JArray ?? throw new Exception("Missing hurt list.");

    foreach (JObject? hurt in hurtList) {
      if (hurt is null) {
        throw new Exception("Null hurt item.");
      }

      // Damage taken.
      int victim_unique_id = (int?)hurt["victim_unique_id"] ?? throw new Exception("Missing victim unique ID.");
      decimal damage = (decimal?)hurt["damage"] ?? throw new Exception("Missing damage.");
      GetRatingData(victim_unique_id).DamageTaken += damage;

      // Damage dealt.
      JObject damageCause = hurt["damage_cause"] as JObject ?? throw new Exception("Missing damage cause.");
      int damageCauseKind = (int?)damageCause["kind"] ?? throw new Exception("Missing damage cause kind.");

      if (damageCauseKind == 1) {
        int attackerUniqueId = (int?)damageCause["attacker_unique_id"] ?? throw new Exception("Missing attacker unique ID.");
        GetRatingData(attackerUniqueId).DamageDealt += damage;
      }
    }
  }
}
