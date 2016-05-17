using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace App1.TrelloTaskManagement
{
	public class TrelloManager
	{
		private static string api_key = "1cfb7de264995091cd0b025fb9aaa1e3";
		private static string token = "5b0b76205a90471ddd0d872a87ee1bd369e4e21fd8efc191833142651ed7058a";
		private static HttpClient cl = new HttpClient();
		HttpResponseMessage msg;

		public async Task<MemberJsonModel> GetCurrentUserInfo()
		{
			msg = await cl.GetAsync($"https://api.trello.com/1/members/me?key={api_key}&token={token}");
			return JsonConvert.DeserializeObject<MemberJsonModel>(await msg.Content.ReadAsStringAsync());
		}

		public async Task<List<ListJsonModel>> GetListsFromBoard(string boardid)
		{
			msg = await cl.GetAsync($"https://api.trello.com/1/boards/{boardid}/lists?key={api_key}&token={token}");
			string rz = await msg.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<List<ListJsonModel>>(rz);
		}

		public async Task<string> GetBoardNameById(string boardId)
		{
			msg = await cl.GetAsync($"https://api.trello.com/1/boards/{boardId}/?key={api_key}&token={token}");
			var result = JsonConvert.DeserializeObject<BoardJsonModel.Rootobject>(await msg.Content.ReadAsStringAsync());
			return result.name;
		}

		public async Task CreateCard(string cardname, string idList, string desc)
		{
			await cl.PostAsync($"https://api.trello.com/1/cards/?key={api_key}&token={token}&idList={idList}&name=" + cardname + "&desc=" + desc, new StringContent(""));
		}


		public async Task CreateList(string listname, string boardId)
		{
			await cl.PostAsync($"https://api.trello.com/1/lists?key={api_key}&token={token}&idBoard={boardId}&name=" + listname, new StringContent(""));
		}

		public async Task CreateBoard(string boardname)
		{
			await cl.PostAsync($"https://api.trello.com/1/boards?key={api_key}&token={token}&name=" + boardname, new StringContent(""));
		}

		public async Task TransferList(string card, string list)
		{
			await cl.PutAsync($"https://api.trello.com/1/cards/{card}?idList={list}&key={api_key}&token={token}", new StringContent(""));
		}

		public async Task UpdateCard(string card, string newname)
		{
			await cl.PutAsync($"https://api.trello.com/1/cards/{card}/?name={newname}&key={api_key}&token={token}", new StringContent(""));
		}

		public async Task<ListJsonModel> GetListById(string list)
		{
			msg = await cl.GetAsync($"https://api.trello.com/1/lists/{list}?key={api_key}&token={token}");
			string rz = await msg.Content.ReadAsStringAsync();

			return JsonConvert.DeserializeObject<ListJsonModel>(rz);
		}

		public async Task<List<CardJsonModel>> GetCardsFromList(string listId)
		{

			msg = await cl.GetAsync($"https://api.trello.com/1/lists/{listId}/cards?key={api_key}&token={token}");
			string rz = await msg.Content.ReadAsStringAsync();


			return JsonConvert.DeserializeObject<List<CardJsonModel>>(rz);
		}

		public async Task DeleteCard(string cardId)
		{
			msg = await cl.DeleteAsync($"https://api.trello.com/1/cards/{cardId}?key={api_key}&token={token}");
		}
	}
}
