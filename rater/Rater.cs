using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace ThuCraft.Rater {
  /// <summary>
  /// Rater rates records.
  /// </summary>
  public class Rater {
    public struct Rating {
      public decimal Base => 1000m;
      public decimal Action { get; set; }
      public decimal Survival { get; set; }
      public decimal Exploration { get; set; }
      public decimal Creation { get; set; }
      public decimal Total => Base * Action * Survival * Exploration * Creation;
    }

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
    public void AddRecord(JsonNode record) {
      try {
        string identifier = record["identifier"]?.ToString() ?? throw new Exception("Missing record identifier.");
        JsonNode recordData = record["data"] ?? throw new Exception("Missing record data.");

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

    private void HandleAfterBlockChange(JsonNode recordData) {
      // TODO: Implement.
    }

    private void HandleAfterEntityDespawn(JsonNode recordData) {
      JsonArray DespawnList = recordData["despawn_list"] as JsonArray ?? throw new Exception("Missing despawn list.");

      foreach (JsonObject? despawn in DespawnList) {
        if (despawn is null) {
          throw new Exception("Null despawn item.");
        }

        int uniqueId = (int?)despawn["unique_id"] ?? throw new Exception("Missing unique ID.");
        GetRatingData(uniqueId).Deaths++;
      }
    }

    private void HandleAfterEntityHurt(JsonNode recordData) {
      JsonArray hurtList = recordData["hurt_list"] as JsonArray ?? throw new Exception("Missing hurt list.");

      foreach (JsonObject? hurt in hurtList) {
        if (hurt is null) {
          throw new Exception("Null hurt item.");
        }

        // Damage taken.
        int victim_unique_id = (int?)hurt["victim_unique_id"] ?? throw new Exception("Missing victim unique ID.");
        decimal damage = (decimal?)hurt["damage"] ?? throw new Exception("Missing damage.");
        GetRatingData(victim_unique_id).DamageTaken += damage;

        // Damage dealt.
        JsonObject damageCause = hurt["damage_cause"] as JsonObject ?? throw new Exception("Missing damage cause.");
        int damageCauseKind = (int?)damageCause["kind"] ?? throw new Exception("Missing damage cause kind.");

        if (damageCauseKind == 1) {
          int attackerUniqueId = (int?)damageCause["attacker_unique_id"] ?? throw new Exception("Missing attacker unique ID.");
          GetRatingData(attackerUniqueId).DamageDealt += damage;
        }
      }
    }
  }
}
