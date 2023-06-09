using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

/// <summary>
/// Rater rates records.
/// </summary>
public class Rater {
  private Dictionary<int, RatingData> _ratingData = new Dictionary<int, RatingData>(); // uid -> rating data

  private int _recordNumber = 0;

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
        case "after_agent_register":
          HandleAfterAgentRegister(recordData);
          break;

        case "after_entity_break_block":
          HandleAfterEntityBreakBlock(recordData);
          break;

        case "after_entity_despawn":
          HandleAfterEntityDespawn(recordData);
          break;

        case "after_entity_heal":
          HandleAfterEntityHeal(recordData);
          break;

        case "after_entity_hurt":
          HandleAfterEntityHurt(recordData);
          break;

        case "after_entity_place_block":
          HandleAfterEntityPlaceBlock(recordData);
          break;

        case "after_entity_position_change":
          HandleAfterEntityPositionChange(recordData);
          break;

        case "after_player_inventory_change":
          HandleAfterPlayerInventoryChange(recordData);
          break;

        case "after_receive_message":
          HandleAfterReceiveMessage(recordData);
          break;

        default:
          break;
      }

    } catch (Exception e) {
      int? tick = (int?)record["tick"];
      throw new Exception($"Bad record (#{_recordNumber}) at tick {tick}: {e.Message}", e);
    }

    _recordNumber++;
  }

  /// <summary>
  /// Gets the rating.
  /// </summary>
  /// <param name="uniqueId">The unique ID.</param>
  /// <returns>The rating.</returns>
  public Rating GetRating(int uniqueId) {
    RatingData ratingData = GetRatingData(uniqueId);
    HashSet<int> itemTypesGot = ratingData.ItemTypesGot;
    return new Rating {
      Token = ratingData.Token,
      Action = 1 + 9 * (decimal)Math.Tanh(0.0001 * ratingData.ActionsOfPerform + ratingData.ActionsOfPing),

      Survival = (1 + 0.3m * ratingData.Kills + 0.02m * ratingData.DamageDealt
                    + 0.1m * ratingData.Healed)
               / (1 + 0.2m * ratingData.Deaths),

      Exploration = 1 + 0.5m * (decimal)Math.Log(Math.Max(ratingData.SectionVisistedCount, 1), 2)
                      + 0.05m * ratingData.CoalOreMined + 0.06m * ratingData.IronOreMined
                      + 0.12m * ratingData.GoldOreMined + 0.24m * ratingData.DiamondOreMined
                      + 0.01m * ratingData.LeavesBroken
                      + 0.3m * ratingData.BlockTypesBrokenCount,

      Creation = 1 + 0.1m * ratingData.ItemTypesGotCount
                   + 0.3m * ratingData.BlockTypesPlacedCount
                   + 0.05m * itemTypesGot.Where(s => (new List<int> { 9, 10, 11, 12 }).Contains(s)).Count()
                   + 0.15m * itemTypesGot.Where(s => (new List<int> { 13, 14, 15, 16 }).Contains(s)).Count()
                   + 0.25m * itemTypesGot.Where(s => (new List<int> { 24, 25, 26, 27 }).Contains(s)).Count()
                   + 0.3m * itemTypesGot.Where(s => (new List<int> { 28, 29, 30, 31 }).Contains(s)).Count()
                   + 0.5m * itemTypesGot.Where(s => (new List<int> { 32, 33, 34, 35 }).Contains(s)).Count(),
      Details = ratingData
    };
  }

  /// <summary>
  /// Gets the unique ID list.
  /// </summary>
  /// <returns>The unique ID list.</returns>
  public List<int> GetUniqueIdList() {
    return (from pair in _ratingData where !(pair.Value.Token is null) select pair.Key).ToList();
  }

  private RatingData GetRatingData(int uniqueId) {
    if (!_ratingData.ContainsKey(uniqueId)) {
      _ratingData.Add(uniqueId, new RatingData());
    }

    return _ratingData[uniqueId];
  }

  private void HandleAfterAgentRegister(JObject recordData) {
    JArray AgentList = recordData["agent_list"] as JArray ?? throw new Exception("Missing agent list.");

    foreach (JObject agent in AgentList) {
      string token = agent["token"]?.ToString() ?? throw new Exception("Missing agent token.");
      string name = agent["name"]?.ToString() ?? throw new Exception("Missing agent name.");
      int uniqueId = (int?)agent["unique_id"] ?? throw new Exception("Missing agent unique ID.");

      if ((from uid in GetUniqueIdList() where GetRatingData(uid).Token == token select uid).Count() > 0) {
        throw new Exception($"Token {token} already used.");
      }

      GetRatingData(uniqueId).Token = token;
      GetRatingData(uniqueId).Name = name;
    }
  }

  private void HandleAfterEntityBreakBlock(JObject recordData) {
    int entityUniqueId = (int?)recordData["entity_unique_id"] ?? throw new Exception("Missing entity unique ID.");
    int blockTypeId = (int?)recordData["block_type_id"] ?? throw new Exception("Missing block type.");

    GetRatingData(entityUniqueId).BlockTypesBroken.Add(blockTypeId);

    switch (blockTypeId) {
      case 16:
        GetRatingData(entityUniqueId).CoalOreMined++;
        break;

      case 15:
        GetRatingData(entityUniqueId).IronOreMined++;
        break;

      case 14:
        GetRatingData(entityUniqueId).GoldOreMined++;
        break;

      case 56:
        GetRatingData(entityUniqueId).DiamondOreMined++;
        break;

      case 18:
        GetRatingData(entityUniqueId).LeavesBroken++;
        break;
    }
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

  private void HandleAfterEntityHeal(JObject recordData) {
    int entityUniqueId = (int?)recordData["entity_unique_id"] ?? throw new Exception("Missing entity unique ID.");
    decimal healAmount = (decimal?)recordData["heal_amount"] ?? throw new Exception("Missing heal amount.");

    if (healAmount < 0) {
      throw new Exception("Negative heal amount.");
    }

    GetRatingData(entityUniqueId).Healed += healAmount;
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

      if (damage < 0) {
        throw new Exception("Negative damage.");
      }

      GetRatingData(victim_unique_id).DamageTaken += damage;

      // Damage dealt and kills.
      JObject damageCause = hurt["damage_cause"] as JObject ?? throw new Exception("Missing damage cause.");
      int damageCauseKind = (int?)damageCause["kind"] ?? throw new Exception("Missing damage cause kind.");

      if (damageCauseKind == 1) {
        int attackerUniqueId = (int?)damageCause["attacker_unique_id"] ?? throw new Exception("Missing attacker unique ID.");
        GetRatingData(attackerUniqueId).DamageDealt += damage;

        bool isDead = (bool?)hurt["is_dead"] ?? throw new Exception("Missing death flag.");
        if (isDead) {
          GetRatingData(attackerUniqueId).Kills++;
        }
      }
    }
  }

  private void HandleAfterEntityPlaceBlock(JObject recordData) {
    int entityUniqueId = (int?)recordData["entity_unique_id"] ?? throw new Exception("Missing entity unique ID.");
    int blockTypeId = (int?)recordData["block_type_id"] ?? throw new Exception("Missing block type.");

    GetRatingData(entityUniqueId).BlockTypesPlaced.Add(blockTypeId);
  }

  private void HandleAfterEntityPositionChange(JObject recordData) {
    JArray changeList = recordData["change_list"] as JArray ?? throw new Exception("Missing change list.");

    foreach (JObject? change in changeList) {
      if (change is null) {
        throw new Exception("Null change item.");
      }

      int uniqueId = (int?)change["unique_id"] ?? throw new Exception("Missing unique ID.");
      int x = (int)((decimal?)change["position"]?["x"] ?? throw new Exception("Missing x.")) / 16 * 16;
      int y = (int)((decimal?)change["position"]?["y"] ?? throw new Exception("Missing y.")) / 16 * 16;
      int z = (int)((decimal?)change["position"]?["z"] ?? throw new Exception("Missing z.")) / 16 * 16;

      GetRatingData(uniqueId).SectionVisisted.Add((x, y, z));
    }
  }

  private void HandleAfterPlayerInventoryChange(JObject recordData) {
    JArray changeList = recordData["change_list"] as JArray ?? throw new Exception("Missing change list.");

    foreach (JObject? change in changeList) {
      if (change is null) {
        throw new Exception("Null change item.");
      }

      int playerUniqueId = (int?)change["player_unique_id"] ?? throw new Exception("Missing player unique ID.");
      JArray slotChangeList = change["change_list"] as JArray ?? throw new Exception("Missing slot change list.");

      foreach (JObject? slotChange in slotChangeList) {
        if (slotChange is null) {
          throw new Exception("Null slot change item.");
        }

        int? item_type_id = (int?)slotChange["item_type_id"];

        if (item_type_id is null) {
          continue;
        }

        GetRatingData(playerUniqueId).ItemTypesGot.Add(item_type_id.Value);
      }
    }
  }

  private void HandleAfterReceiveMessage(JObject recordData) {
    int playerUniqueId = (int?)recordData["unique_id"] ?? throw new Exception("Missing player unique ID.");

    int typeCode = (int?)(recordData["message"]?["type"]) ?? throw new Exception("Missing message type.");

    if (typeCode == 100) {
      GetRatingData(playerUniqueId).ActionsOfPing++;

    } else if (typeCode >= 300 && typeCode < 400) {
      GetRatingData(playerUniqueId).ActionsOfGet++;

    } else if (typeCode >= 500 && typeCode < 600) {
      GetRatingData(playerUniqueId).ActionsOfPerform++;
    }
  }
}
