using System;
using System.Collections.Generic;

namespace Voltage.Witches.Models
{
    using Voltage.Witches.User;
//    using Voltage.Witches.Models.Avatar;
    using Voltage.Witches.Models.Avatar;

	// NOTE: Maybe reverse and make Player the decorator...or just fix the interfaces

    /* Intended to wrap all IPlayer calls with a serialize method. should eventually be turned into a full decorator */
    public class PersistentPlayer : Player
    {
        public PersistentPlayer(List<Spellbook> books, PlayerDataStore dataStore, IPlayerWriter playerWriter, 
            PlayerStaminaManager staminaManager, PlayerFocusManager focusManager) 
            : base(dataStore, books, playerWriter, staminaManager, focusManager)
        {
        }

        public override void SetPlayerName(string firstName, string lastName)
        {
            base.SetPlayerName(firstName, lastName);
            Serialize();
        }

        public override void StartScene(string sceneId)
        {
            base.StartScene(sceneId);
            Serialize();
        }

        public override void UpdateSceneProgress(string scene, string id)
        {
            base.UpdateSceneProgress(scene, id);
            Serialize();
        }

        public override void UpdateHowTosSceneProgress(string sceneID, string id)
        {
            base.UpdateHowTosSceneProgress(sceneID, id);
            Serialize();
        }
        

        public override void AddAvailableScene(string scene)
        {
            base.AddAvailableScene(scene);
            Serialize();
        }

        public override void RemoveScene(string scene)
        {
            base.RemoveScene(scene);
            Serialize();
        }

        protected override void HandleSceneComplete(bool success, Action<bool> onComplete)
        {
            base.HandleSceneComplete(success, onComplete);

            if (success)
            {
                Serialize();
            }
        }

		public override void CompleteRoute ()
		{
			base.CompleteRoute ();
			Serialize ();
		}

        public override void UpdateFocus()
        {
            base.UpdateFocus();
            Serialize();
        }

        public override void DeductFocus()
        {
            base.DeductFocus();
            Serialize();
        }

        public override void UpdateCurrency(int amount)
        {
            base.UpdateCurrency(amount);
            Serialize();
        }

        public override void UpdatePremiumCurrency(int amount)
        {
            base.UpdatePremiumCurrency(amount);
            Serialize();
        }

		public override void UpdateStaminaPotion(int amount)
		{
			base.UpdateStaminaPotion (amount);
			Serialize ();
		}


        public override void UpdateInventory(string id, int amount)
        {
            base.UpdateInventory(id, amount);
            Serialize();
        }

        public override void UpdateOutfit(Outfit newOutfit)
        {
            base.UpdateOutfit(newOutfit);
            Serialize();
        }

        public override void SaveOutfitPreset(string name, Outfit outfit)
        {
            base.SaveOutfitPreset(name, outfit);
            Serialize();
        }

		public override void IncreaseBitProgress()
		{
			base.IncreaseBitProgress();
			Serialize();
		}

        public override void DeductStamina()
        {
            base.DeductStamina();
            Serialize();
        }

        public override void UpdateStamina()
        {
			base.UpdateStamina();
            Serialize();
        }

		public override void ExchangePotionForStamina ()
		{
			base.ExchangePotionForStamina ();
			Serialize ();
		}

        public override void AddClosetSpace()
        {
            base.AddClosetSpace();
            Serialize();
        }


		public override void AddBook(ISpellbook book)
		{
			base.AddBook (book);
			Serialize ();
		}

		public override void UpdateMiniGameProgress (string recipeID, CompletionStage level)
		{
			base.UpdateMiniGameProgress (recipeID, level);
			Serialize ();
		}

		public override void SetTutorialProgress(int step, string name)
		{
			base.SetTutorialProgress (step, name);
			Serialize ();
		}

		#region Ambient tutorial hack
		public override void SetAvatarTutorialProgress(int step, string name)
		{
			base.SetAvatarTutorialProgress (step, name);
			Serialize ();
		}
		public override void StartAvatarTutorial()
		{
			base.StartAvatarTutorial();
			Serialize ();
		}

		public override void FinishAvatarTutorial()
		{
			base.FinishAvatarTutorial();
			Serialize ();
		}
		#endregion

		public override void FinishTutorial()
		{
			base.FinishTutorial ();
			Serialize ();
		}

		public override void RefillStamina()
		{	base.RefillStamina ();
			Serialize ();
		}

		public override void TrackCurrentSceneAffectedCharacters(string initial, int affinity)
		{
			base.TrackCurrentSceneAffectedCharacters (initial, affinity);
			Serialize ();
		}
		
		public override void DestroyTrackingForCurrentAffinities()
		{
			base.DestroyTrackingForCurrentAffinities ();
			Serialize ();
		}

		public override bool NotificationsEnabled {
			set {
				base.NotificationsEnabled = value;
				Serialize();
			}
		}


        public override void MakeStarterPackAvailable(double durationInDays)
        {
            base.MakeStarterPackAvailable(durationInDays);
            Serialize();
        }

        public override void PurchaseStarterPack()
        {
            base.PurchaseStarterPack();
            Serialize();
        }

        public override void AwardedBonusItem()
        {
            base.AwardedBonusItem();
            Serialize();
        }

        public override bool ShouldPromptUserForNotifications
        {
            set {
                base.ShouldPromptUserForNotifications = value;
                Serialize();
            }
        }

    }
}

