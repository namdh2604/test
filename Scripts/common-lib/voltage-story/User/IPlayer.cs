using System;

namespace Voltage.Story.User	// TODO: this "interface" should be moved into the witches namespace
{
    using Voltage.Witches.Models;
    using Voltage.Common.Collections;

    public interface IPlayer
    {
        void SetPlayerName(string firstName, string lastName);

		// Story
        void StartScene(string scene);
        void UpdateSceneProgress(string scene, string id);
        void AddAvailableScene(string scene);
        void RemoveScene(string scene);
        void CompleteScene(Action<bool> onComplete);

		void AddBook(ISpellbook book);

		void UpdateStaminaPotion(int difference);
        void UpdatePremiumCurrency(int amount);
        void UpdateCurrency(int amount);
		event EventHandler PotionUpdate;
		event EventHandler CurrencyUpdate;

        ReadOnlyDictionary<string, int> GetInventoryData();
        void UpdateInventory(string id, int amount);

        void Serialize();

        void AddClosetSpace();

        int Stamina { get; }
        bool IsStaminaMaxed();
        DateTime StaminaNextUpdate { get; }
        void DeductStamina();
        void UpdateStamina();
        void Refresh();
        event EventHandler StaminaUpdate;

		void ExchangePotionForStamina();


		// MiniGame
//		void UpdateMiniGameProgress(IRecipe recipe, CompletionStage level);	
		void UpdateMiniGameProgress(string recipeID, CompletionStage level);
//		void AddIngredient(...)
//		void DeductIngredient (...)

        int Focus { get; }
        bool IsFocusMaxed();
        DateTime FocusNextUpdate { get; }
        void DeductFocus();
        void UpdateFocus();
        event EventHandler FocusUpdate;
    }
    
}



