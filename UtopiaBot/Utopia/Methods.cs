using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using xNet;

namespace UtopiaBot.Utopia
{
    class Methods
    {
		public class QueryFilter
		{
			public string sortBy = "";
			public string offset = "";
			public string limit = "";

			public JObject toJObject()
			{
				return JObject.FromObject(this);
			}
		}

		public static string apiQuery(string token, string host, string method = "getSystemInfo", JObject params_obj = null, QueryFilter filter = null)
		{
			JObject json_obj = new JObject();
			if (params_obj == null)
			{
				params_obj = new JObject();
			}

			json_obj.Add(new JProperty("method", method));
			json_obj.Add(new JProperty("params", params_obj));
			json_obj.Add(new JProperty("token", token));
			if (filter != null)
			{
				json_obj.Add(new JProperty("filter", filter.toJObject()));
			}

			string jsonQuery = json_obj.ToString();

			//http(s)://127.0.0.1:port/api/1.0
			string result = Utopia.HttpRequest.buildJsonQuery("http://" + host + "/api/1.0/", "POST", jsonQuery);
			return result;
		}


		public static int CheckValid(string host, string token)
        {
			/*Status codes:
			 * 200 - Авторизация успешна
			 * 300 - Невозможно подключится к серверу
			 * 400 - Токен не верный
			 * 500 - Неизвестная ошибка
			 */
			try
			{
				string[] _host = host.Split(':');
				using (TcpClient tcpClient = new TcpClient())
				{
					try
					{
						tcpClient.Connect(_host[0], Convert.ToInt32(_host[1]));
					}
					catch (Exception)
					{
						Console.WriteLine("Port closed");
						return 300;
					}
				}

				var json = JsonConvert.DeserializeObject<Utopia.SystemInfo.Root>(apiQuery(token, host));
				if (json.error != null)
				{
					return 400;
				}
				else
				{
					return 200;
				}
			}
			catch
			{
				return 500;
			}
        }
		private static string getError(int code)
		{
			string body = "";
			switch (code)
			{
				case 300:
					body = "\u274cНевозможно подключится к серверу" +
						"\n\n" +
						"*Список возможных причин:*" +
						"\n- Вы неправильно настроили режим API" +
						"\n- Порт API - закрыт";
					break;
				case 400:
					body = "\u274cВведеный токен - не верный";
					break;
				case 500:
					body = "\u274cНеизвестная ошибка" +
						"\n\n" +
						"*Обратитесь к администратору*";
					break;
				default:
					body = "\u274cНеизвестная ошибка" +
						"\n\n" +
						"*Обратитесь к администратору*";
					break;
			}
			return body;
		}

		public static string getAccountInfo(string host, string token)
		{
			try
			{
				int check = CheckValid(host, token);
				if (check != 200)
				{
					return getError(check);
				}
				else
				{
					var bal = JsonConvert.DeserializeObject<Utopia.getBalance.Root>(apiQuery(token, host, "getBalance"));
					var status = JsonConvert.DeserializeObject<Utopia.getProfileStatus.Root>(apiQuery(token, host, "getProfileStatus"));
					var sysinfo = JsonConvert.DeserializeObject<Utopia.SystemInfo.Root>(apiQuery(token, host));
					var owncontact = JsonConvert.DeserializeObject<Utopia.getOwnContact.Root>(apiQuery(token, host, "getOwnContact"));

					string _status = "";
					switch (status.result.status_code)
					{
						case 4096:
							_status = "В сети";
							break;
						case 4097:
							_status = "Нет на месте";
							break;
						case 4099:
							_status = "Не беспокоить";
							break;
						case 32768:
							_status = "Неведимка";
							break;
						case 65536:
							_status = "Не в сети";
							break;
						default:
							_status = "\u274cОшибка!";
							break;
					}
					string body =
						"\ud83e\udd35*Информация аккаунта:*" +
						"\n├ Ник: `" + owncontact.result.nick + "`" +
						"\n├ Статус: `" + _status + "`" +
						"\n└ Баланс: `" + bal.result + " CRP`" +
						"\n" +
						"\n\ud83d\udda5*Системная информация:*" +
						"\n├ Версия сборки: `" + sysinfo.result.build_number + "`" +
						"\n├ Архетиктура CPU: `" + sysinfo.result.currentCpuArchitecture + "`" +
						"\n├ Кол-во подключений: `" + sysinfo.result.numberOfConnections + "`" +
						"\n└ Время работы: `" + sysinfo.result.uptime + "`";
					return body;
				}
			}
			catch
			{
				string body = "\u274cНеизвестная ошибка" +
						"\n\n" +
						"*Обратитесь к администратору*";
				return body;
			}
		}

		public static string setStatus(string host, string token, string id)
		{
			try
			{
				int check = CheckValid(host, token);
				if (check != 200)
				{
					return getError(check);
				}
				else
				{
					string _status = "Available";
					switch (id)
					{
						case "1":
							_status = "Available";
							break;
						case "2":
							_status = "Away";
							break;
						case "3":
							_status = "DoNotDisturb";
							break;
						case "4":
							_status = "Invisible";
							break;
						case "5":
							_status = "Offline";
							break;
					}
					JObject params_obj = new JObject();
					params_obj.Add(new JProperty("status", _status));
					params_obj.Add(new JProperty("mood", ""));
					var setstatus = JsonConvert.DeserializeObject<Utopia.setStatus.Root>(apiQuery(token, host, "setProfileStatus", params_obj));
					if (setstatus.result == true)
					{
						string body = "\u2705Статус успешно изменен на: " + _status;
						return body;
					}
					else
					{
						string body = "\u274cОшибка изменения статуса!";
						return body;
					}
				}
			}
			catch
			{
				string body = 
					"\u274cНеизвестная ошибка" +
					"\n\n" +
					"*Обратитесь к администратору*";
				return body;
			}
		}

		public static string getCards(string host, string token, string action, string arg = null)
		{
			/*try
			{*/
				int check = CheckValid(host, token);
				if (check != 200)
				{
					return getError(check);
				}
				else
				{
					switch (action)
					{
						case "list":
							var cards = JsonConvert.DeserializeObject<Utopia.getCards.Root>(apiQuery(token, host, "getCards"));
							string _cards = "";
							if (cards.result.Count <= 0)
							{
								_cards = "\u274c*Карт не найдено!*";
							}
							else
							{
								for (int i = 0; i < cards.result.Count; i++)
								{
									DateTime date = DateTime.Parse(cards.result[i].created);
									_cards += (i + 1) + ". " + cards.result[i].name +
										"\n├ Баланс: `" + cards.result[i].balance + " CRP`" +
										"\n├ ID: `" + cards.result[i].cardid + "`" +
										"\n└ Создана: `"+ date.ToString("dd.MM.yyyy HH:mm:ss") + "`" +
										"\n";
								}
							}
							string body =
								"\ud83d\udcb3Список ваших карт(" + cards.result.Count + "):" +
								"\n" +
								"\n" + _cards;
							return body;
						case "del":
							if (arg == "")
							{
								return "\u274cВы не передали _ID карты_!";
							}
							else
							{
								JObject params_obj = new JObject();
								params_obj.Add(new JProperty("cardId", arg));
								var delCard = JsonConvert.DeserializeObject<Utopia.deleteCard.Root>(apiQuery(token, host, "deleteCard", params_obj));
								if (delCard.result != null)
								{
									return "\u2705Карта успешно удалена!";
								}
								else 
								{
									return "\u274cОшибка удаления карты!";
								}

							}
						case "add":
							if (arg == "")
							{
								return "\u274cВы не передали _Название карты_!";
							}
							else
							{
								JObject params_obj = new JObject();
								params_obj.Add(new JProperty("name", arg));
								params_obj.Add(new JProperty("color", "#FFFFFF"));
								var addCard = JsonConvert.DeserializeObject<Utopia.AddCard.Root>(apiQuery(token, host, "addCard", params_obj));
								if (addCard.result != null)
								{
									return "\u2705Карта успешно создана!";
								}
								else
								{
									return
										"\u274cОшибка создания карты!" +
										"\n" +
										"\n*Возможно, у вас не хватает средств*";
								}
							}
						default:
							return "\u274cНеизвестное действие!";
					}
					
				}
			/*}
			catch
			{
				string body =
				"\u274cНеизвестная ошибка" +
				"\n\n" +
				"*Обратитесь к администратору*";
				return body;
			}*/
		}

		public static string getFinanceSystemInformation(string host, string token)
		{
			try
			{
				int check = CheckValid(host, token);
				if (check != 200)
				{
					return getError(check);
				}
				else
				{
					var cards = JsonConvert.DeserializeObject<Utopia.getFinanceSystemInformation.Root>(apiQuery(token, host, "getFinanceSystemInformation"));

					string body =
						"\ud83e\udd11*Финансовая информация:*" +
						"\n" +
						"\n\ud83d\udcb3*Карты*" +
						"\n├ Возможность создания: `" + cards.result.cardsCreationEnabled + "`" +
						"\n├ Цена создания: `" + cards.result.cardCreatePrice + " CRP`" +
						"\n└ Макс. количество в день/активных: `" + cards.result.cardsMaxPerDay + "/" + cards.result.cardsMaxActive + "`" +
						"\n" +
						"\n\ud83d\udcc3*Ваучеры*" +
						"\n├ Возможность создания: `" + cards.result.vouchersCreateEnabled + "`" +
						"\n├ Возможность использования: `" + cards.result.vouchersUseEnabled + "`" +
						"\n└ Макс. количество за один раз/активных: `" + cards.result.vouchersMaxPerBatch + "/" + cards.result.vouchersMaxActive + "`";
					return body;
				}
			}
			catch
			{
				string body =
				"\u274cНеизвестная ошибка" +
				"\n\n" +
				"*Обратитесь к администратору*";
				return body;
			}
		}

		public static string getContacts(string host, string token, string action, string pb_key = null)
		{
			try
			{
				int check = CheckValid(host, token);
				if (check != 200)
				{
					return getError(check);
				}
				else
				{
					switch (action) 
					{
						case "list":
							var contacts = JsonConvert.DeserializeObject<Utopia.getContacts.Root>(apiQuery(token, host, "getContacts"));
							string _contacts = "";
							if (contacts.result.Count <= 0)
							{
								_contacts = "\u274c*Контактов не найдено!*";

							}
							else
							{
								for (int i = 0; i < contacts.result.Count; i++)
								{
									string group = "Нет";
									if (contacts.result[i].group != "")
									{
										group = contacts.result[i].group;
									}
									string _status = "";
									switch (contacts.result[i].status)
									{
										case 4096:
											_status = "В сети";
											break;
										case 4097:
											_status = "Нет на месте";
											break;
										case 4099:
											_status = "Не беспокоить";
											break;
										case 32768:
											_status = "Неведимка";
											break;
										case 65536:
											_status = "Не в сети";
											break;
										default:
											_status = "\u274cОшибка!";
											break;
									}
									_contacts += (i + 1) + "." + contacts.result[i].nick +
										"\n├ Публичный ключ: `" + contacts.result[i].pk + "`" +
										"\n├ Группа: `" + group + "`" +
										"\n├ Друг: `" + (contacts.result[i].isFriend ? "Да" : "Нет") + "`" +
										"\n└ Статус: `" + _status + "`" +
										"\n\n";
								}
							}
							string body =
								"\ud83d\ude4b\u200d\u2642\ufe0fСписок ваших контактов(" + contacts.result.Count + "):" +
								"\n" +
								"\n" + _contacts;
							return body;
						case "del":
							if (pb_key == "")
							{
								return "\u274cВы не передали _публичный ключ_!";
							}
							else
							{
								JObject params_obj = new JObject();
								params_obj.Add(new JProperty("pk", pb_key));
								var delContact = JsonConvert.DeserializeObject<Utopia.deleteContact.Root>(apiQuery(token, host, "deleteContact", params_obj));
								if(delContact.result == true)
								{
									return "\u2705Контакт успешно удален!";
								}
								else
								{
									return "\u274cОшибка удаления контакта!";
								}
							}
						case "ava":
							if (pb_key == null)
							{
								return "\u274cВы не передали _публичный ключ_!";
							}
							else
							{
								JObject params_obj = new JObject();
								params_obj.Add(new JProperty("pk", pb_key));
								params_obj.Add(new JProperty("coder", "BASE64"));
								params_obj.Add(new JProperty("format", "JPG"));
								var delContact = JsonConvert.DeserializeObject<Utopia.getContactAvatar.Root>(apiQuery(token, host, "getContactAvatar", params_obj));
								if (delContact.result != null)
								{
									return delContact.result;
								}
								else
								{
									return "*err*";
								}
							}
						default:
							return "\u274cНеизвестное действие!";
							
					}

				}
			}
			catch
			{
				string body =
				"\u274cНеизвестная ошибка" +
				"\n\n" +
				"*Обратитесь к администратору*";
				return body;
			}
		}

	}
}
