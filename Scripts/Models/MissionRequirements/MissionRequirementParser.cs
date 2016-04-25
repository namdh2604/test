using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Voltage.Witches.Models.MissionRequirements
{
    using Voltage.Witches.Exceptions;

    public interface IMissionRequirementParser
    {
        IMissionRequirement Parse(string input);
    }

    public class MissionRequirementParser : IMissionRequirementParser
    {
        private Regex _affinityPattern = new Regex(@"^\s*Characters/([^/]+)/Affinity\s+([><]?)=\s+(\d+)\s*$");
        private Regex _scenePattern = new Regex(@"^\s*Selections/(.+)/\w+\s*$");

        public MissionRequirementParser()
        {
        }

        public IMissionRequirement Parse(string input)
        {
            Match possibleMatch = _affinityPattern.Match(input);
            if (possibleMatch.Success)
            {
				var characterName = possibleMatch.Groups[1].Value;
				var affinity = (int)Convert.ToSingle(possibleMatch.Groups[(possibleMatch.Groups.Count - 1)].Value);
	
				return new AffinityRequirement(characterName, affinity);
            }
            else if (ClothingRequirement.HandlesRequirement(input))
            {
                return ClothingRequirement.Create(input);
            }

            possibleMatch = _scenePattern.Match(input); 
            if (possibleMatch.Success)
            {
                return new ProgressRequirement(possibleMatch.Groups[1].Value);
            }

            throw new WitchesException("Unrecognized Mission Requirement: " + input);
        }
    }
}

