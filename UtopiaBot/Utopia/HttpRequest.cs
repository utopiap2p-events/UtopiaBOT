using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UtopiaBot.Utopia
{
    class HttpRequest
    {
		public static string buildJsonQuery(string url, string method = "GET", string body = "")
		{
			//SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | 
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
			WebRequest request = WebRequest.Create(url);
			request.Method = method;
			//request.Accept = "application/json";
			request.ContentType = "application/json";

			if (body != "")
			{
				byte[] requestBody = Encoding.UTF8.GetBytes(body);
				request.ContentLength = requestBody.Length;
				using (Stream stream = request.GetRequestStream())
				{
					stream.Write(requestBody, 0, requestBody.Length);
				}
			}

			HttpWebResponse response;
			try
			{
				response = (HttpWebResponse)request.GetResponse();
			}
			catch (WebException exception)
			{
				response = (HttpWebResponse)exception.Response;
			}

			string responseBody = "";
			using (Stream stream = response.GetResponseStream())
			{
				using (StreamReader streamReader = new StreamReader(stream))
				{
					responseBody = streamReader.ReadToEnd();
				}
			}
			return responseBody;
		}
	}
}
