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
using App1.TrelloTaskManagement;

namespace Bot_Application1
{
	public class NamedEntity
	{
		public string Id { get; set; }
		public string Name { get; set; }
	}

	public class Board : NamedEntity
	{
		public List<TrelloList> Lists { get; set; } = new List<TrelloList>();
	}

	public class TrelloList : NamedEntity
	{
		public List<Card> Cards { get; set; } = new List<Card>();
	}
	public class Card : NamedEntity
	{
		public string Description { get; set; }
	}

	[BotAuthentication]
	public class MessagesController : ApiController
	{
		HttpClient client = new HttpClient();
		TrelloManager manager = new TrelloManager();

		public async Task<LUISResponse> CallLUIS(string query)
		{
			var response = await client.GetAsync("https://api.projectoxford.ai/luis/v1/application?id=195e744b-2759-49d2-969d-03094df90fc7&subscription-key=f1fb570966e343d6a3be33ce9acf1fe0&q=" + query);
			var content = await response.Content.ReadAsStringAsync();
			var LUISResponse = JsonConvert.DeserializeObject<LUISResponse>(content);
			return LUISResponse;
		}

		static bool cachedBoards = false;
		static List<Board> cache = new List<Board>();

		public async Task<Message> Post([FromBody]Message message)
		{
			if(!cachedBoards || message.Text == "clear_cache")
			{
				await CacheBoards();
				cachedBoards = true;
				if(message.Text == "clear_cache")
					return message.CreateReplyMessage("Cache cleared");
			}

			if (message.Type == "Message")
			{
				var response = await CallLUIS(message.Text);
				switch (response.intents[0].intent)
				{
					case "Greetings":
						return message.CreateReplyMessage(new Random().NextDouble() > 0.5 ? "Hi!" : "Hello");

					case "Create":
						{
							string card = response.entities.SingleOrDefault(x => x.type == "Card")?.entity;
							string list = response.entities.SingleOrDefault(x => x.type == "List")?.entity;
							string board = response.entities.SingleOrDefault(x => x.type == "Board")?.entity;
							string description = response.entities.SingleOrDefault(x => x.type == "Description")?.entity ?? string.Empty;

							if (card == null && list == null && board == null)
								return message.CreateReplyMessage("What do you want to create?");

							if (card != null && list != null && board != null)
							{
								//Create new card with description
								var tBoard = cache.FirstOrDefault(x => x.Name.ToLower() == board.ToLower());
								if (tBoard == null)
									return message.CreateReplyMessage("Board with this name cannot be found");
								var tlist = tBoard.Lists.FirstOrDefault(x => x.Name.ToLower() == list.ToLower());
								if (tlist == null)
									return message.CreateReplyMessage("List with this name cannot be found");
								await manager.CreateCard(card, tlist.Id, description);
								var cards = await manager.GetCardsFromList(tlist.Id);
								var ids = tlist.Cards.Select(x => x.Id);

								foreach (var carte in cards)
									if (ids.Contains(carte.id) == false)
										tlist.Cards.Add(new Card() { Id = carte.id, Name = carte.name, Description = carte.desc });
								
								return message.CreateReplyMessage($"Card {card} created in list {list} on board {board} with " + (description == string.Empty ? "empty description" : "description " + description));
							}
							else if (list == null)
								return message.CreateReplyMessage("List wasn't specified");
							else if (board == null)
								return message.CreateReplyMessage("Board wasn't specified");

							if (card == null && list != null && board != null)
							{
								//Create new list
								var id = cache.First(x => x.Name.ToLower() == board.ToLower()).Id;
								await manager.CreateList(list, id);

								var lists = await manager.GetListsFromBoard(id);
								var brd = cache.Single(x => x.Id == id);
								var ids = brd.Lists.Select(x => x.Id);

								foreach (var item in lists)
									if(ids.Contains(item.id) == false)
										brd.Lists.Add(new TrelloList() { Id = item.id, Name = item.name });


								return message.CreateReplyMessage($"List {list} created on board {board}.");
							}
							else if (board == null)
								return message.CreateReplyMessage("Board wasn't specified");

							if (card == null && list == null && board != null)
							{
								await manager.CreateBoard(board);

								var member = await manager.GetCurrentUserInfo();
								var ids = cache.Select(x => x.Id);
								foreach (var item in member.idBoards)
								{
									if (ids.Contains(item) == false)
										cache.Add(new Board() { Id = item, Name = board, Lists = new List<TrelloList>() });
								}

								return message.CreateReplyMessage($"Board {board} created.");
							}
						}
						break;
					case "Update":
						{
							string card = response.entities.SingleOrDefault(x => x.type == "Card")?.entity;
							string cardToUpdate = response.entities.SingleOrDefault(x => x.type == "CardToUpdate")?.entity;
							if (card == null || cardToUpdate == null)
								break;
							var c = cache.SelectMany(x => x.Lists.SelectMany(y => y.Cards)).FirstOrDefault(x => x.Name.ToLower() == cardToUpdate.ToLower());
							c.Name = card;
							await manager.UpdateCard(c.Id, c.Name);
						}
						break;
					case "Read":
						{
							string card = response.entities.SingleOrDefault(x => x.type == "Card")?.entity;
							if(card != null)
							{
								var desc = cache.SelectMany(x => x.Lists.SelectMany(y => y.Cards)).FirstOrDefault(x => x.Name.ToLower() == card.ToLower())?.Description;
								if (desc != null)
									return message.CreateReplyMessage($"Card {card} description: \r\n{desc}");
							}
						}
						break;

					case "Finish":
						{
							string card = response.entities.SingleOrDefault(x => x.type == "Card")?.entity;
							if (card == null)
								break;
							var cardId = cache.SelectMany(x => x.Lists.SelectMany(y => y.Cards)).FirstOrDefault(x => x.Name.ToLower() == card.ToLower())?.Id;
							var listId = cache.SelectMany(x => x.Lists).FirstOrDefault(x => x.Name.ToLower() == "done")?.Id;
							if (cardId != null && listId != null)
							{
								await manager.TransferList(cardId, listId);
								return message.CreateReplyMessage($"I've moved card {card} to Done List");
							}

						}
						break;

					case "Delete":
						{
							string card = response.entities.SingleOrDefault(x => x.type == "Card")?.entity;
							if (card == null)
								break;
							var c = cache.SelectMany(x => x.Lists.SelectMany(y => y.Cards)).FirstOrDefault(x => x.Name.ToLower() == card.ToLower());
							var lst = cache.SelectMany(x => x.Lists).Single(x => x.Cards.Contains(c));
							lst.Cards.Remove(c);
							await manager.DeleteCard(c.Id);
							return message.CreateReplyMessage($"{card} deleted from list {lst.Name}");
						}
					default:
						return message.CreateReplyMessage("Can you elaborate on that?");
				}

				return message.CreateReplyMessage("Your request seems messed up a bit.");
			}
			else
			{
				return HandleSystemMessage(message);
			}
		}

		private async Task CacheBoards()
		{
			cache.Clear();
			var user = await manager.GetCurrentUserInfo();
			foreach (var item in user.idBoards)
			{
				var name = await manager.GetBoardNameById(item);
				var lists = await manager.GetListsFromBoard(item);
				List<TrelloList> trelloLists = new List<TrelloList>();
				foreach (var list in lists)
				{
					var lst = new TrelloList() { Id = list.id, Name = list.name };
					var cards = await manager.GetCardsFromList(lst.Id);
					lst.Cards = cards.Select(x => new Card() { Id = x.id, Name = x.name, Description = x.desc }).ToList();
					trelloLists.Add(lst);
				}
				cache.Add(new Board() { Name = name, Lists = trelloLists, Id = item });
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