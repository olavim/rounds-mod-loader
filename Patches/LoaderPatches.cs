using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using RoundsModLoader.Cards;

namespace RoundsModLoader.Patches
{
    [HarmonyPatch(typeof(CardChoice), "Awake")]
    class CardChoice_Patch
    {
        static void Postfix()
        {
            // fetch card to use as a template for all custom cards
            ModLoader.templateCard = (from c in CardChoice.instance.cards
                                      where c.cardName.ToLower() == "huge"
                                      select c).FirstOrDefault();

            ModLoader.defaultCards = CardChoice.instance.cards;
            ModLoader.moddedCards.AddRange(ModLoader.defaultCards);
        }
    }

    [HarmonyPatch(typeof(ApplyCardStats), "ApplyStats")]
    class ApplyCardStats_Patch
    {
        static void Prefix(ApplyCardStats __instance, Player ___playerToUpgrade)
        {
            var player = ___playerToUpgrade.GetComponent<Player>();
            var gun = ___playerToUpgrade.GetComponent<Holding>().holdable.GetComponent<Gun>();
            var characterData = ___playerToUpgrade.GetComponent<CharacterData>();
            var healthHandler = ___playerToUpgrade.GetComponent<HealthHandler>();
            var gravity = ___playerToUpgrade.GetComponent<Gravity>();
            var block = ___playerToUpgrade.GetComponent<Block>();
            var gunAmmo = gun.GetComponentInChildren<GunAmmo>();
            var characterStatModifiers = player.GetComponent<CharacterStatModifiers>();

            CustomCard customAbility = __instance.gameObject.GetComponent<CustomCard>();
            if (customAbility != null)
            {
                customAbility.OnAddCard(player, gun, gunAmmo, characterData, healthHandler, gravity, block, characterStatModifiers);
            }
        }
    }
    
    [HarmonyPatch(typeof(CardBarHandler), "AddCard")]
    class CardBarHandler_Patch
    {
        static void Prefix(int teamId, CardInfo card)
        {
            ModLoader.BuildInfoPopup("Added Card: " + card.cardName);
            CardData.AddCard(teamId, card.cardName);
        }
    }
}