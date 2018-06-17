﻿namespace CustomCraftSML.Serialization
{
    using System.Collections.Generic;
    using EasyMarkup;
    using SMLHelper.Patchers;
    using UnityEngine.Assertions;

    public class ModifiedRecipe : EmPropertyCollection, IModifiedRecipe
    {
        public const short Max = 25;
        public const short Min = 1;

        protected readonly EmProperty<TechType> emTechType;
        protected readonly EmProperty<short> amountCrafted;
        protected readonly EmPropertyCollectionList<EmIngredient> ingredients;
        protected readonly EmPropertyList<TechType> linkedItems;

        public TechType ItemID => emTechType.Value;

        public short AmountCrafted
        {
            get
            {
                Assert.IsTrue(amountCrafted.Value <= Max, $"Amount crafted value for {ItemID} must be less than {Max}.");
                Assert.IsTrue(amountCrafted.Value >= Min, $"Amount crafted value for {ItemID} must be greater than {Min}.");
                return amountCrafted.Value;
            }
        }

        public List<TechType> LinkedItems => linkedItems.Values;

        public readonly List<Ingredient> Ingredients = new List<Ingredient>();

        public static List<EmProperty> ModifiedRecipeProperties => new List<EmProperty>(4)
        {
            new EmProperty<TechType>("ItemID"),
            new EmProperty<short>("AmountCrafted", 1),
            new EmPropertyCollectionList<EmIngredient>("Ingredients", new EmIngredient()),
            new EmPropertyList<TechType>("LinkedItemIDs")
        };

        public ModifiedRecipe() : this("ModifiedRecipe", ModifiedRecipeProperties)
        {
        }

        public ModifiedRecipe(string key) : this(key, ModifiedRecipeProperties)
        {
        }

        protected ModifiedRecipe(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            emTechType = (EmProperty<TechType>)Properties["ItemID"];
            amountCrafted = (EmProperty<short>)Properties["AmountCrafted"];
            ingredients = (EmPropertyCollectionList<EmIngredient>)Properties["Ingredients"];
            linkedItems = (EmPropertyList<TechType>)Properties["LinkedItemIDs"];
                        
            OnValueExtractedEvent += ValueExtracted;
        }

        private void ValueExtracted()
        {
            foreach (var ingredient in ingredients.Collections)
            {
                TechType itemID = (ingredient["ItemID"] as EmProperty<TechType>).Value;
                short required = (ingredient["Required"] as EmProperty<short>).Value;

                Ingredients.Add(new Ingredient(itemID, required));
            }
        }

        internal override EmProperty Copy() => new ModifiedRecipe(Key, CopyDefinitions);

        public virtual TechDataHelper SmlHelperRecipe()
        {
            var ingredientsList = new List<IngredientHelper>(Ingredients.Count);

            foreach (Ingredient item in Ingredients)
            {
                ingredientsList.Add(new IngredientHelper(item.ItemID, item.Required));
            }

            return new TechDataHelper()
            {
                _techType = ItemID,
                _craftAmount = AmountCrafted,
                _linkedItems = LinkedItems,
                _ingredients = ingredientsList
            };
        }
    }
}
