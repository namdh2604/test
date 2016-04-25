using System;

namespace Voltage.Story.Models.Nodes.Helpers
{
    public class CharacterAttribute : ICloneable, IEquatable<CharacterAttribute>
    {
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string Outfit { get; set; }
        public string Pose { get; set; }
        public string Expression { get; set; }  // TODO: string

        public CharacterAttribute()
        {
            Enabled = false;
            Name = string.Empty;
            Outfit = string.Empty;
            Pose = string.Empty;
            Expression = string.Empty;
        }

        public object Clone()
        {
            return this.MemberwiseClone();  
        }

        public override string ToString ()
        {
            return string.Format ("[CharacterAttribute: Enabled={0}, Name={1}, Outfit={2}, Pose={3}, Expression={4}]", Enabled, Name, Outfit, Pose, Expression);
        }

        public bool Equals(CharacterAttribute other)
        {
            return ((Enabled == other.Enabled) &&
            (Name == other.Name) &&
            (Outfit == other.Outfit) &&
            (Pose == other.Pose) &&
            (Expression == other.Expression));
        }
    }
}

