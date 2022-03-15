using System;
using System.IO;
using System.Media;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace rezerwacje.duw.pl
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Start:
            string mainUrl = "https://rezerwacje.duw.pl/reservations/pol/login";
            string myUrl;
            Console.Write("Enter your login/email:");
            string loginUserInput = Console.ReadLine();
            Console.Write("Enter your password:");
            string passwordUserInput = Console.ReadLine();
            string login = HttpUtility.UrlEncode(loginUserInput);
            string password = HttpUtility.UrlEncode(passwordUserInput);
            while (true)
            {
                for (int i = 0; i < 16; i++)
                {
                    CookieContainer cookies = new CookieContainer();
                    DateTime date = DateTime.Today.AddDays(i);
                    string dateString = date.ToString("yyyy-MM-dd");
                    myUrl = $"https://rezerwacje.duw.pl/reservations/pol/queues/108/29/{dateString}";

                    string OdpGet = FirstGet(mainUrl, cookies);
                    var token = Regex.Match(OdpGet, "(?<=name=\"data\\[_Token]\\[key]\" value=\").*?(?=\")");

                    if (!string.IsNullOrEmpty(token.Value))
                    {
                        string OdpPost = LoginPost(mainUrl, login, password, HttpUtility.HtmlEncode(token.Value), cookies);

                        string firstContain = "Brak wolnych terminów.";
                        string secondContain = "<title>Errors :: </title>";
                        string thirdContain = "Server fail";
                        string fourthContain = "<title>Zaloguj :: </title>";

                        //wyswietla odp dla proby zalogowania
                        if (OdpPost.Contains(firstContain))
                        {
                            Console.WriteLine($"{firstContain} - {dateString} - {DateTime.Now}");
                        }
                        else if (OdpPost.Contains(secondContain))
                        {
                            Console.WriteLine($"{secondContain} - {dateString} - {DateTime.Now}");
                        }
                        else if (OdpPost.Contains(thirdContain))
                        {
                            Console.WriteLine($"{thirdContain} - {dateString} - {DateTime.Now}");
                        }
                        else if (OdpPost.Contains(fourthContain))
                        {
                            Console.WriteLine($"Wrong login of password");
                            goto Start;
                        }
                        else
                        {
                            while (true)
                            {
                                PlaySound();
                                Thread.Sleep(5000);
                            }
                        }

                        //wyswietla cookies dla podanej domeny
                        //foreach (var item in cookies.GetCookies(new Uri(shortUrl)))
                        //{
                        //    Console.WriteLine(item.ToString());
                        //}
                    }
                    else
                    {
                        Console.WriteLine("Can't find token");
                    }
                }
                Thread.Sleep(60000);
            }


            string FirstGet(string _url, CookieContainer _cookie)
            {
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(_url);
                    request.Method = "GET";
                    //request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.4844.51 Safari/537.36";
                    //request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
                    //request.Headers.Add("Accept-Language", "en-US,en;q=0.9,pl;q=0.8");
                    //request.Headers.Add("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\"");
                    //request.Headers.Add("sec-ch-ua-mobile", "?0");
                    //request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                    //request.Headers.Add("Upgrade-Insecure-Requests", "1");
                    //request.Headers.Add("Sec-Fetch-Site", "none");
                    //request.Headers.Add("Sec-Fetch-Mode", "navigate");
                    //request.Headers.Add("Sec-Fetch-User", "?1");
                    //request.Headers.Add("Sec-Fetch-Dest", "document");
                    request.CookieContainer = _cookie;

                    WebResponse response = request.GetResponse();

                    string responseFromServer = string.Empty;

                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(dataStream);
                        responseFromServer = reader.ReadToEnd();
                    }

                    response.Close();

                    return responseFromServer;

                }
                catch (Exception error)
                {

                    Console.WriteLine(error.Message);
                    return "aaaaaaaaaaaaaaaaaaaa";
                }
            }

            string LoginPost(string _url, string _login, string _haslo, string _token, CookieContainer _cookie)
            {
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(_url);
                    request.Method = "POST";
                    //request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.4844.51 Safari/537.36";
                    //request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
                    //request.Referer = mainUrl;
                    //request.Headers.Add("Origin", "https://rezerwacje.duw.pl");
                    //request.Headers.Add("Accept-Language", "en-US,en;q=0.9,pl;q=0.8");
                    //request.Headers.Add("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"99\", \"Google Chrome\";v=\"99\"");
                    //request.Headers.Add("sec-ch-ua-mobile", "?0");
                    //request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                    //request.Headers.Add("Upgrade-Insecure-Requests", "1");
                    //request.Headers.Add("Sec-Fetch-Site", "none");
                    //request.Headers.Add("Sec-Fetch-Mode", "navigate");
                    //request.Headers.Add("Sec-Fetch-User", "?1");
                    //request.Headers.Add("Sec-Fetch-Dest", "document");
                    request.CookieContainer = _cookie;

                    string postData = $"_method=POST&data%5B_Token%5D%5Bkey%5D={_token}&data%5BUser%5D%5Bemail%5D={_login}&data%5BUser%5D%5Bpassword%5D={_haslo}&data%5BUser%5D%5Breturn_to%5D=&data%5B_Token%5D%5Bfields%5D=950d15a2cfde63e0ea9e1fa1b85587c9ff982433%253AUser.return_to&data%5B_Token%5D%5Bunlocked%5D=";
                    byte[] byteArray = Encoding.UTF8.GetBytes(postData);

                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = byteArray.Length;

                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();

                    WebResponse response = request.GetResponse();

                    string responseFromServer;

                    using (dataStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(dataStream);
                        responseFromServer = reader.ReadToEnd();
                    }
                    response.Close();




                    var requestFinalPage = (HttpWebRequest)WebRequest.Create(myUrl);
                    requestFinalPage.Method = "GET";
                    requestFinalPage.CookieContainer = _cookie;
                    string responseFromServerFinalPage;
                    WebResponse responseFinalPage = requestFinalPage.GetResponse();
                    using (dataStream = responseFinalPage.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(dataStream);
                        responseFromServerFinalPage = reader.ReadToEnd();
                    }
                    responseFinalPage.Close();
                    return responseFromServerFinalPage;
                }
                catch (Exception)
                {
                    Console.WriteLine("Internet connection failed");
                    return "Server fail";
                }


            }


            void PlaySound()
            {

                SoundPlayer player = new SoundPlayer(@"C:/Users/Kamil/source/repos/Games/Games/ConsoleApp2/mixkit-street-public-alarm-997.wav");
                player.Play();

            }
        }
    }
}
