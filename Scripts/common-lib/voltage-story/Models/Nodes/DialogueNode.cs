using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

using Voltage.Story.Models.Nodes.ID;
using Voltage.Story.Models.Nodes.Extensions;

namespace Voltage.Story.Models.Nodes
{
    using Voltage.Story.Models.Nodes.Helpers;

	public class DialogueNode : BaseNode, IHaveText, IHasCharacters, IHasBackdrop, IHasSpeaker, IHasMusic
    {
		public string Text { get; private set; }
		public string Speaker { get; private set; }
		public string Music
		{ 
			get 
			{
				string value = string.Empty;
				_data.TryGetValue("music", out value);
				return value;
			}
			set
			{
				string key = "music";
				if(_data.ContainsKey(key))
				{
					_data[key] = value;
				}
				else
				{
					_data.Add(key, value);
				}
			}
		}

		public string SpeechBox
		{ 
			get 
			{
				string value = string.Empty;
				_data.TryGetValue("speechBox", out value);
				return value;
			}
			set
			{
				string key = "speechBox";
				if(_data.ContainsKey(key))
				{
					_data[key] = value;
				}
				else
				{
					_data.Add(key, value);
				}
			}
		}

		protected IList<CharacterAttribute> _characters = new List<CharacterAttribute> { new CharacterAttribute(), new CharacterAttribute() };

		public CharacterAttribute LeftCharacter
		{
			get
			{
				if(_characters.Count > 0)
				{
					return _characters[0];
				}
				else
				{
					throw new IndexOutOfRangeException();
				}
			}
			set
			{
				if(_characters.Count > 0)
				{
					_characters[0] = value;
				}
				else
				{
					throw new IndexOutOfRangeException();
				}
			}
		}

		public CharacterAttribute RightCharacter
		{
			get
			{
				if(_characters.Count > 1)
				{
					return _characters[1];
				}
				else
				{
					throw new IndexOutOfRangeException();
				}
			}
			set
			{
				if(_characters.Count > 1)
				{
					_characters[1] = value;
				}
				else
				{
					throw new IndexOutOfRangeException();
				}
			}
		}


		public DialogueNode(JToken json, INode caller, IIDGenerator<string,INode> idGenerator) : base (json, caller, idGenerator) 
		{
			DefaultAttributes();

			SetAttributes(json);

            Text = TryGet (json, "text", string.Empty);
            Speaker = TryGet (json, "speaker", string.Empty);

            SetBackground(json["data"]);
            SetMusic(json["data"]);

			SetCharacters(json);
		}

		private void DefaultAttributes ()
		{
			_data = new Dictionary<string,string>
			{
				{"speechBox", string.Empty},
				{"text", string.Empty}
			};
		}

        private void SetBackground(JToken data)
        {
            this.Background = GetBackground(Previous, data);
        }

        private string GetBackground(INode previous, JToken data)
        {
            if ((data != null) && (data["background"] != null))
            {
                return data.Value<string>("background");
            }

            IHasBackdrop node = previous.FindNodeBefore(x => x is IHasBackdrop) as IHasBackdrop;
            if (node == null)
            {
                throw new Exception("No valid background found!");
            }

            return node.Background;
        }

        private void SetMusic(JToken data)
        {
            this.Music = GetMusic(Previous, data);
        }

        public static string GetMusic(INode previous, JToken data)
        {
            if ((data != null) && (data["music"] != null))
            {
                return data.Value<string>("music");
            }

            IHasMusic candidate = previous.FindNodeBefore(x => x is IHasMusic) as IHasMusic;

            // HACK -- Selection nodes initialize their children before finishing their own initialization, which means the children don't inherit the parents values
            // this bypasses the selection nodes and just tries to find a previous node of that selection node that has music information
            // note that this means that selection nodes themselves cannot clear music
            while ((candidate != null) && (string.IsNullOrEmpty(candidate.Music)) && (candidate is SelectionNode))
            {
                SelectionNode buggedNode = candidate as SelectionNode;
                if (buggedNode.Previous == null)
                {
                    break;
                }

                candidate = buggedNode.Previous.FindNodeBefore(x => x is IHasMusic) as IHasMusic;
            }

            if (candidate == null)
            {
                return string.Empty;
            }

            return candidate.Music;
        }

		private void SetCharacters (JToken json)
		{
			if(json != null)
			{
				JToken data = json["data"];
                JToken left = null;
                JToken right = null;
                if (data != null)
                {
                    left = data["left"];
                    right = data["right"];
                }
                ConfigureCharacter(left, () => LeftCharacter, (node) => LeftCharacter = DuplicateChar(node.LeftCharacter));
                ConfigureCharacter(right, () => RightCharacter, (node) => RightCharacter = DuplicateChar(node.RightCharacter));
			}
		}

        private CharacterAttribute DuplicateChar(CharacterAttribute charData)
        {
            return (charData == null) ? new CharacterAttribute() : (CharacterAttribute)(charData.Clone());
        }

        private void ConfigureCharacter (JToken charJson, Func<CharacterAttribute> getCharacter, Action<IHasCharacters> assignCharacterAttributeTo)
		{
            IHasCharacters characterNode = Previous.FindNodeBefore(x => x is IHasCharacters) as IHasCharacters;
            if (characterNode != null)
			{
                assignCharacterAttributeTo(characterNode);
			}

			if(charJson != null)
			{
				CharacterAttribute character = getCharacter ();	// NOTE: must get reference after prior assignment

				try
				{
					character.Enabled = TryGet<bool>(charJson, "enabled", character.Enabled);

					character.Name = TryGet<string>(charJson, "name", character.Name);

					character.Outfit = TryGet<string>(charJson, "outfit", character.Outfit);

					character.Pose = TryGet<string>(charJson, "pose", character.Pose);

					character.Expression = TryGet<string>(charJson, "expression", character.Expression);

				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}

		public override string ToString ()
		{
			return string.Format ("[{0}]dialogue({1})\n{2}", ID, Text, Next != null ? Next.ToString () : "null");
		}
    }
}
