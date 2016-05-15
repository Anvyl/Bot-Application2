
namespace Bot_Application1
{

	public class LUISResponse
	{
		public Intent[] intents { get; set; }
		public Entity[] entities { get; set; }
	}

	public class Intent
	{
		public string intent { get; set; }
		public float score { get; set; }
	}

	public class Entity
	{
		public string entity { get; set; }
		public string type { get; set; }
	}

}