using System;
using System.Web;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace WD.Library.Core
{
	/// <summary>
	/// �ṩ��Uri��ش������غ�����������þ�̬��������ʽ�ṩ��Uri�еĲ�����ȡ��Uri�����ȹ��ܡ� 
	/// </summary>
	public static class UriHelper
	{
		#region Public
		/// <summary>
		/// ����Url���õ����еĲ�������
		/// </summary>
		/// <param name="url">Uri���͵�Url������·�������·��</param>
		/// <returns>NameValueCollection����������</returns>
		public static NameValueCollection GetUriParamsCollection(Uri url)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(url != null, "url");

			return GetUriParamsCollection(url.ToString());
		}

		/// <summary>
		/// ��url�У���ȡ�����ļ���
		/// </summary>
		/// <param name="uriString">url</param>
		/// <returns>��������</returns>
		public static NameValueCollection GetUriParamsCollection(string uriString)
		{
			return GetUriParamsCollection(uriString, true);
		}

		/// <summary>
		/// ��url�У���ȡ�����ļ���
		/// </summary>
		/// <param name="uriString">url</param>
		/// <param name="urlDecode">�Ƿ�ִ��decode</param>
		/// <returns>��������</returns>
		public static NameValueCollection GetUriParamsCollection(string uriString, bool urlDecode)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(uriString != null, "uriString");

			NameValueCollection result = new NameValueCollection(StringComparer.OrdinalIgnoreCase);

			string bookmarkString = GetBookmarkStringInUrl(uriString);

			if (bookmarkString != string.Empty)
				uriString = uriString.Remove(uriString.Length - bookmarkString.Length, bookmarkString.Length);

			string query = uriString;

			int startIndex = query.IndexOf("?");

			if (startIndex >= 0)
				query = query.Substring(startIndex + 1);

			if (query != string.Empty)
			{
				string[] parts = query.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

				for (int i = 0; i < parts.Length; i++)
				{
					int equalsSignIndex = parts[i].IndexOf("=");

					string paramName = string.Empty;
					string paramValue = string.Empty;

					if (equalsSignIndex >= 0)
					{
						//���ڵȺ�
						paramName = parts[i].Substring(0, equalsSignIndex);
						paramValue = parts[i].Substring(equalsSignIndex + 1);
					}

					if (string.IsNullOrEmpty(paramName) == false)
					{
						if (urlDecode)
						{
							paramName = HttpUtility.UrlDecode(paramName);
							paramValue = HttpUtility.UrlDecode(paramValue);
						}

						AddValueToCollection(paramName, paramValue, result);
					}
				}
			}

			return result;
		}

		/// <summary>
		/// ��Url�еĲ����������򣬷��ز���������url��
		/// </summary>
		/// <param name="url">Uri���͵�Url������·�������·��</param>
		/// <returns>����������url��</returns>
		public static string GetUrlWithSortedParams(Uri url)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(url != null, "url");

			return GetUrlWithSortedParams(url.ToString());
		}

		/// <summary>
		/// ��Url�еĲ����������򣬷��ز���������url��
		/// </summary>
		/// <param name="uriString">Url������·�������·��</param>
		/// <returns>����������url��</returns>
		public static string GetUrlWithSortedParams(string uriString)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(uriString != null, "uriString");

			string query = string.Empty;
			string leftPart = string.Empty;

			int startIndex = uriString.IndexOf("?");

			if (startIndex >= 0)
			{
				leftPart = uriString.Substring(0, startIndex) + "?";
				query = uriString.Substring(startIndex + 1);
			}
			else
				leftPart = uriString;

			StringBuilder strB = new StringBuilder(2048);

			if (query != string.Empty)
			{
				NameValueCollection paramCollection = GetUriParamsCollection(query);
				string[] allKeys = paramCollection.AllKeys;

				Array.Sort(allKeys, System.Collections.CaseInsensitiveComparer.Default);

				for (int i = 0; i < allKeys.Length; i++)
				{
					string key = allKeys[i];

					if (strB.Length > 0)
						strB.Append("&");

					strB.Append(HttpUtility.UrlEncode(key));

					if (key != string.Empty)
						strB.Append("=");

					strB.Append(HttpUtility.UrlEncode(paramCollection[key]));
				}
			}

			return leftPart + strB.ToString();
		}

		/// <summary>
		/// �Ƴ�Url��ָ���Ĳ���
		/// </summary>
		/// <param name="uriString"></param>
		/// <param name="paramNames"></param>
		/// <returns></returns>
		public static string RemoveUrlParams(string uriString, params string[] paramNames)
		{
			return RemoveUrlParams(uriString, Encoding.UTF8, paramNames);
		}

		/// <summary>
		/// �Ƴ�Url��ָ���Ĳ���
		/// </summary>
		/// <param name="uriString"></param>
		/// <param name="encoding"></param>
		/// <param name="paramNames"></param>
		/// <returns></returns>
		public static string RemoveUrlParams(string uriString, Encoding encoding, params string[] paramNames)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(uriString != null, "uriString");
			ExceptionHelper.FalseThrow<ArgumentNullException>(encoding != null, "encoding");
			ExceptionHelper.FalseThrow<ArgumentNullException>(paramNames != null, "paramNames");

			StringBuilder strB = new StringBuilder(1024);

			string leftPart = string.Empty;

			int startIndex = uriString.IndexOf("?");

			if (startIndex >= 0)
				leftPart = uriString.Substring(0, startIndex);
			else
				leftPart = uriString;

			NameValueCollection originalParams = GetUriParamsCollection(uriString);

			foreach(string paramName in paramNames)
			{
				if (originalParams[paramName] != null)
					originalParams.Remove(paramName);
			}

			return CombineUrlParams(uriString, encoding, originalParams);
		}

		/// <summary>
		/// ������������ϳ�Url
		/// </summary>
		/// <param name="uriString">url</param>
		/// <param name="encoding">�ַ�����</param>
		/// <param name="requestParamsArray">�������ϵ�����</param>
		/// <returns>�����˲�����url</returns>
		public static string CombineUrlParams(string uriString, Encoding encoding, params NameValueCollection[] requestParamsArray)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(uriString != null, "uriString");
			ExceptionHelper.FalseThrow<ArgumentNullException>(encoding != null, "encoding");
			ExceptionHelper.FalseThrow<ArgumentNullException>(requestParamsArray != null, "requestParamsArray");

			NameValueCollection requestParams = MergeParamsCollection(requestParamsArray);

			StringBuilder strB = new StringBuilder(1024);

			string leftPart = string.Empty;

			int startIndex = uriString.IndexOf("?");

			if (startIndex >= 0)
				leftPart = uriString.Substring(0, startIndex);
			else
				leftPart = uriString;

			for (int i = 0; i < requestParams.Count; i++)
			{
				if (i == 0)
					strB.Append("?");
				else
					strB.Append("&");

				strB.Append(HttpUtility.UrlEncode(requestParams.Keys[i], encoding));
				strB.Append("=");
				strB.Append(HttpUtility.UrlEncode(requestParams[i], encoding));
			}

			return leftPart + strB.ToString();
		}

		/// <summary>
		/// ������������ϳ�Url
		/// </summary>
		/// <param name="uriString">url</param>
		/// <param name="requestParamsArray">�������ϵ�����</param>
		/// <returns>�����˲�����url</returns>
		public static string CombineUrlParams(string uriString, params NameValueCollection[] requestParamsArray)
		{
			return CombineUrlParams(uriString, Encoding.UTF8, requestParamsArray);
		}

		/// <summary>
		/// �õ�url�е���ǩ���֡���#������Ĳ���
		/// </summary>
		/// <param name="queryString">http://localhost/lianhome#littleTurtle</param>
		/// <returns>littleTurtle</returns>
		public static string GetBookmarkStringInUrl(string queryString)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(queryString != null, "queryString");

			int bookmarkStart = -1;

			for (int i = queryString.Length - 1; i >= 0; i--)
			{
				if (queryString[i] == '#')
					bookmarkStart = i;
				else
					if (queryString[i] == '&' || queryString[i] == '?')
						break;
			}

			string result = string.Empty;

			if (bookmarkStart >= 0)
				result = queryString.Substring(bookmarkStart);

			return result;
		}

		/// <summary>
		/// ����Uri�����UriΪ���·��������Uri��~�������滻Ϊ��ǰ��WebӦ��
		/// </summary>
		/// <param name="uriString">Uri</param>
		/// <returns>���UriΪ���·��������Uri��~�������滻Ϊ��ǰ��WebӦ��</returns>
		public static Uri ResolveUri(string uriString)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(uriString != null, "uriString");

			Uri url = new Uri(uriString, UriKind.RelativeOrAbsolute);

			if (url.IsAbsoluteUri == false && string.IsNullOrEmpty(uriString) == false)
			{
				if (EnvironmentHelper.Mode == InstanceMode.Web)
				{
					if (uriString[0] == '~')
					{
						HttpRequest request = HttpContext.Current.Request;
						string appPathAndQuery = request.ApplicationPath + uriString.Substring(1);

						appPathAndQuery = appPathAndQuery.Replace("//", "/");

						uriString = request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped) +
									appPathAndQuery;

						url = new Uri(uriString);
					}
				}
			}

			return url;
		}
		#endregion

		#region Private
		private static NameValueCollection MergeParamsCollection(NameValueCollection[] requestParams)
		{
			NameValueCollection result = new NameValueCollection(StringComparer.OrdinalIgnoreCase);

			for (int i = 0; i < requestParams.Length; i++)
				MergeTwoParamsCollection(result, requestParams[i]);

			return result;
		}

		private static void MergeTwoParamsCollection(NameValueCollection target, NameValueCollection src)
		{
			foreach (string key in src.Keys)
			{
				if (target[key] == null)
					target.Add(key, src[key]);
			}
		}

		private static void AddValueToCollection(string paramName, string paramValue, NameValueCollection result)
		{
			string oriValue = result[paramName];

			if (oriValue == null)
				result.Add(paramName, paramValue);
			else
			{
				string rValue = oriValue;

				if (oriValue.Length > 0)
					rValue += ",";

				rValue += paramValue;

				result[paramName] = rValue;
			}
		}
		#endregion
	}
}
