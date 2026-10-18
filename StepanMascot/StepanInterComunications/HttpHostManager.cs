using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

namespace StepanMascot.StepanInterComunications
{
    public class HttpHostManager
    {
        HttpListener _listener = new HttpListener();

        public HttpHostManager()
        {
            _listener.Prefixes.Add("http://localhost:2424/");
        }

        public async Task Start()
        {
            _listener.Start();

            while (mascotGraphicManager.running && _listener.Prefixes.Count > 0)
            {
                var context = _listener.GetContext();

                if (context != null)
                    await ManageContext(context);
            }

            _listener.Stop();
            _listener.Close();
        }

        private async Task ManageContext(HttpListenerContext context)
        {
            var response = context.Response;
            var request = context.Request;

            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

            Console.WriteLine(request?.Url?.AbsolutePath);

            if (request.HttpMethod == "OPTIONS")
            {
                response.StatusCode = 200;
                response.OutputStream.Close();
                return;
            }

            if (request.HttpMethod == "POST")
            {
                #region Mascot Management
                if (request.Url.AbsolutePath == "/mascotcontroller/create")
                {
                    //ler o conteudo
                    string jsonContent;
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        jsonContent = reader.ReadToEnd();

                        JObject obj = JObject.Parse(jsonContent);

                        ActionsQueue.shareActions.Enqueue(() => mascotGraphicManager.mascotsController.CreateMascot(obj));
                        
                    }
                }
                if (request.Url.AbsolutePath == "/mascotcontroller/add")
                {
                   
                }
                if (request.Url.AbsolutePath == "/mascotcontroller/remove")
                {
                    
                }
                if (request.Url.AbsolutePath == "/mascotcontroller/render")
                {
                   
                }
                #endregion
            }


            string responseString = "Cara, tamo estudando tamo estudando";
            byte[] buffer = Encoding.UTF8.GetBytes( responseString );

            response.ContentType = "text/plain; charset=utf-8";
            response.ContentLength64 = buffer.Length;

            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length );

            await output.DisposeAsync();

            response.Close();
        }
    }
}
