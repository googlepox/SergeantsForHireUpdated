using System;

namespace Sergeants
{
	public class Config
	{
		public int infantry_sergeant_cost { get; set; }
		public int mounted_sergeant_cost { get; set; }
		public int ranged_sergeant_cost { get; set; }
		public int warden_cost { get; set; }
		public float infantry_sergeant_hire_scale { get; set; }
		public float mounted_sergeant_hire_scale { get; set; }
		public float ranged_sergeant_hire_scale { get; set; }
		public float warden_hire_scale { get; set; }
		public int sergeant_party_size_bonus { get; set; }
		public int lieutenant_bonus_size { get; set; }
		public int warden_prisoner_size_bonus { get; set; }
		public float male_fraction { get; set; }
		public float sergeant_morale_boost_delay { get; set; }
		public float sergeant_morale_boost_amount { get; set; }
		public float lieutenant_morale_boost_factor { get; set; }
	}
}
