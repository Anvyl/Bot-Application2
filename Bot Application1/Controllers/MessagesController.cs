using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Connector;
using System.Net.Http;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.Bot.Builder.FormFlow;
using System;
using TrelloNet;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bot_Application1
{





	[BotAuthentication]
	public class MessagesController : ApiController
	{


		HttpClient client = new HttpClient();


		public async Task<LUISResponse> CallLUIS(string query)
		{
			var response = await client.GetAsync("https://api.projectoxford.ai/luis/v1/application?id=195e744b-2759-49d2-969d-03094df90fc7&subscription-key=f1fb570966e343d6a3be33ce9acf1fe0&q=" + query);
			var content = await response.Content.ReadAsStringAsync();
			var LUISResponse = JsonConvert.DeserializeObject<LUISResponse>(content);
			return LUISResponse;
		}
		
		public async Task<Message> Post([FromBody]Message message)
		{
			if (message.Type == "Message")
			{
				var response = await CallLUIS(message.Text);
				switch (response.intents[0].intent)
				{
					case "Create":
						string card = response.entities.SingleOrDefault(x => x.type == "Card")?.entity;
						string list = response.entities.SingleOrDefault(x => x.type == "List")?.entity;
						string board = response.entities.SingleOrDefault(x => x.type == "Board")?.entity;
						string description = response.entities.SingleOrDefault(x => x.type == "Description")?.entity ?? string.Empty;

						if(card == null && list == null && board == null)
							return message.CreateReplyMessage("What do you want to create?");
						
						if (card != null && list != null && board != null)
						{
							//Create new card with description
							return message.CreateReplyMessage($"Card {card} created in list {list} on board {board} with " + (description == string.Empty ? "empty description" : "description " + description));
						}
						else if (list == null)
							return message.CreateReplyMessage("List wasn't specified");
						else if (board == null)
							return message.CreateReplyMessage("Board wasn't specified");

						if (card == null && list != null && board != null)
						{
							//Create new card with description
							return message.CreateReplyMessage($"List {list} created on board {board}.");
						}
						else if (board == null)
							return message.CreateReplyMessage("Board wasn't specified");

						if (card ==null && list == null && board != null)
						{
							//Create new card with description
							return message.CreateReplyMessage($"Board {board} created.");
						}
						break;
					case "Update":

						break;

					case "Finish":
						break;
					default:
						break;
				}

				// calculate something for us to return
				int length = (message.Text ?? string.Empty).Length;

				// return our reply to the user
				return message.CreateReplyMessage($"You sent {length} characters");
			}
			else
			{
				return HandleSystemMessage(message);
			}
		}

		private Message HandleSystemMessage(Message message)
		{
			if (message.Type == "Ping")
			{
				Message reply = message.CreateReplyMessage();
				reply.Type = "Ping";
				return reply;
			}
			else if (message.Type == "DeleteUserData")
			{
				// Implement user deletion here
				// If we handle user deletion, return a real message
			}
			else if (message.Type == "BotAddedToConversation")
			{
			}
			else if (message.Type == "BotRemovedFromConversation")
			{
			}
			else if (message.Type == "UserAddedToConversation")
			{
			}
			else if (message.Type == "UserRemovedFromConversation")
			{
			}
			else if (message.Type == "EndOfConversation")
			{
			}

			return null;
		}
	}
}