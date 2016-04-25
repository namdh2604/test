using System;
using System.Text.RegularExpressions;

namespace Voltage.Witches.Models.MissionRequirements
{
    using Voltage.Story.Variables;
    using Voltage.Witches.Models.Avatar;
    using Voltage.Witches.Exceptions;

    public class ClothingRequirement : IMissionRequirement
    {
        public OutfitCategory Category { get; private set; }
        public string Piece { get; private set; }

        private static readonly Regex PATTERN = new Regex(@"^\s*Avatar/(\S+)\s+=\s+""(\w+)""\s*$");
        public static bool HandlesRequirement(string candidate)
        {
            return PATTERN.IsMatch(candidate);
        }

        public static ClothingRequirement Create(string rawInput)
        {
            Match match = PATTERN.Match(rawInput);
            if (!match.Success)
            {
                throw new WitchesException("Invalid clothing requirement: " + rawInput);
            }

            OutfitCategory category = ParseCategory(match.Groups[1].Value);
            string itemId = match.Groups[2].Value;

            return new ClothingRequirement(category, itemId);
        }

        private static OutfitCategory ParseCategory(string rawCategory)
        {
            string[] tokens = rawCategory.Split('/');
            if (tokens.Length > 1)
            {
                rawCategory = tokens[tokens.Length - 1];
            }

            try
            {
                return (OutfitCategory)Enum.Parse(typeof(OutfitCategory), rawCategory);
            }
            catch (ArgumentException)
            {
                throw new WitchesException("Invalid outfit category: " + rawCategory);
            }
        }

        public ClothingRequirement(OutfitCategory category, string piece)
        {
            Category = category;
            Piece = piece.Trim();
        }

        public bool Evaluate(VariableMapper context)
        {
            string itemId;
            context.TryGetValue(Category.ToString(), out itemId);

            return (Piece == itemId.Trim());
        }
    }
}

