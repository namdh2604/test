using UnityEngine;
using System.Collections;

namespace Voltage.Witches.Models
{
	public enum AffinityGrade
	{
		NONE = 0,
		SPHERE_1 = 1,
		SPHERE_2 = 2,
		SPHERE_3 = 3,
		SPHERE_4 = 4,
		SPHERE_5 = 5,
		SPHERE_6 = 6,
		SPHERE_7 = 7,
		SPHERE_8 = 8,
		SPHERE_9 = 9
	}

	public class Affinity
	{
		public string AffinityTitle { get; protected set; }
		public AffinityGrade Grade { get; protected set; }
		public int CurrentAffinityScore { get; protected set; }

		public Affinity(string affinity,int grade,int score)
		{
			AffinityTitle = affinity;
			Grade = (AffinityGrade)grade;
			CurrentAffinityScore = score;
		}

		public void UpdateAffinity(int increment)
		{
			CurrentAffinityScore += increment;
			//TODO Something with a map to update the other fields
		}
	}
}