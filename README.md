## Programa desarrollado en Unity que muestra una ruta entre dos ubicaciones en un teléfono Android


### Pasos previos 

* Instalar la versión 2019.3.14f1 de Unity
* Obtener la clave para realizar llamadas al API en https://cloud.google.com/maps-platform?hl=es 
* Si se desea hacer debug en el teléfono Android descargar de Play Store Unity Remote 5

### Creación del programa

1. Iniciar Unity Hub y crear un nuevo programa 3D llamado GetLocationGoogleMaps
2. Ir a File, Build Settings ...
3. Seleccionar Android, dar clic en Switch Platform
4. Ingresar un campo para ingreso de datos dando clic derecho en la escena, UI, InputField y configurarlo de la siguiente manera: 
* Nombre: Origen, Pos X: 80, Pos Y: 195 
5. Ingresar un campo para ingreso de datos dando clic derecho en la escena, UI, InputField y configurarlo de la siguiente manera: 
* Nombre: Destino, Pos X: 80, Pos Y: 165 
6. Ingresar un botón para guardar la información dando clic derecho en la escena, UI, Button y configurarlo de la siguiente manera: 
* Nombre: ObtenerDireccion, Pos X: 80, Pos Y: 135
7. Ingresar una imagen para mostrar las direcciones dando clic derecho en la escena, UI, Raw Image y configurarlo de la siguiente manera: 
* Nombre: Direccion, Pos X: 100, Pos Y: 100
8. Dar clic derecho en Canvas y en la opción Scale Factor colocar 4
9. En la parte inferior, en Assets dar clic derecho y crear la carpeta Scripts
10. En esta carpeta crear un script de C# con el nombre de Directions
11. Copiar el siguiente código en el script
```c#
using System;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class Directions : MonoBehaviour
{
    private string key = "YOUR_API_KEY";

    public InputField origen;
    public InputField destino;
    public RawImage imagen;

    public void GetGoogleMapsInfo()
    {
        string directionsInfo = CallDirectionsInfo(origen.text, destino.text);

        directionsInfo = directionsInfo.Replace(@"\\",@"\");

        CallStaticMap(directionsInfo);
    }

    private string CallDirectionsInfo(string origen, string destino)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format("https://maps.googleapis.com/maps/api/directions/json?origin={0}&destination={1}&key={2}", origen, destino, key));
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();

        int inicio = jsonResponse.LastIndexOf("points") != -1 ? jsonResponse.LastIndexOf("points") : -1;
        string puntos = jsonResponse.Substring(inicio + 11);
        int fin = puntos.IndexOf("\"");

        return puntos.Remove(fin); ;
    }

    private void CallStaticMap(string directionsInfo)
    {
        MemoryStream ms = new MemoryStream();
        Texture2D texture2D = new Texture2D(2, 2);
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://maps.googleapis.com/maps/api/staticmap?size=400x400&path=enc%3A" + directionsInfo + "&key=" + key);

        using (Stream responseStream = request.GetResponse().GetResponseStream())
        {
            byte[] buffer = new byte[0x1000];
            int bytes;

            while ((bytes = responseStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, bytes);
            }
        }

        byte[] bytes2 = ms.ToArray();
        texture2D.LoadImage(bytes2);
        imagen.texture = texture2D;
    }

}

```
12. Una vez copiado el código añadir el script Directions a la camara principal
13. Asociar la función GetGoogleMapsInfo a la funcionalidad OnClick() del botón Guardar
22. Ingresar a File/Build Settings, dar clic  en "Add Open Scenes" y Build
23. Probar

```c#
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using PureCloudPlatform.Client.V2.Api;
using PureCloudPlatform.Client.V2.Client;
using PureCloudPlatform.Client.V2.Extensions;
using PureCloudPlatform.Client.V2.Model;

namespace 
{
    public class CargaAudiosStep
    {
        private void DownloadRecordings()
        {
            var accessTokenInfo = Configuration
                .Default.ApiClient
                .PostToken("USUARIO", "CLAVE");

            var conversationsApi = new ConversationsApi();
            Configuration.Default.AccessToken = accessTokenInfo.AccessToken;

            string path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = path.Substring(0, path.LastIndexOf("bin"));
            System.IO.Directory.CreateDirectory(path + "\\Recordings");

			//Intervalo de consulta
			//string interval = yesterday + "T0:00:00.000Z/" + today + "T0:00:00.000Z";
            string interval = "2020-06-09T0:00:00.000Z/2020-06-10T0:00:00.000Z";

            AnalyticsConversationQueryResponse conversationDetails = conversationsApi.PostAnalyticsConversationsDetailsQuery(new ConversationQuery(Interval: interval));
            
			// Pass conversation details to function
            extractConversationDetails(conversationDetails);

            if (Debugger.IsAttached)
            {
                Console.ReadKey();
            }


        }

        /// <summary>
        /// Format conversation details to object inside and array. Get every mediatype per conversation
        /// </summary>
        /// <param name="conversationDetails"></param>
        /// <returns></returns>
        private void extractConversationDetails(AnalyticsConversationQueryResponse conversationDetails)
        {
            // Push all conversationId from conversationDetails to conversationIds
            foreach (var conversationDetail in conversationDetails.Conversations)
            {
                getRecordingMetaData(conversationDetail.ConversationId);
            }
        }

        /// <summary>
        /// Generate recordingId for every conversationId
        /// </summary>
        /// <param name="conversationId"></param>
        /// <returns></returns>
        private void getRecordingMetaData(string conversationId)
        {
            RecordingApi recordingApi = new RecordingApi();
            List<Recording> recordingsData = recordingApi.GetConversationRecordingmetadata(conversationId);

            // Pass recordingsMetadata to a function
            iterateRecordingsData(recordingsData);
        }

        /// <summary>
        /// Iterate through every result, check if there are one or more recordingIds in every conversation
        /// </summary>
        /// <param name="recordingsData"></param>
        /// <returns></returns>
        private void iterateRecordingsData(List<Recording> recordingsData)
        {
            foreach (var iterateRecordings in recordingsData)
            {
                getSpecificRecordings(iterateRecordings);
            }
        }

        /// <summary>
        /// Plot conversationId and recordingId to request for batchDownload Recordings
        /// </summary>
        /// <param name="iterateRecordings"></param>
        /// <returns></returns>
        private void getSpecificRecordings(Recording iterateRecordings)
        {
            List<BatchDownloadRequest> batchRequest = new List<BatchDownloadRequest>();
            BatchDownloadRequest batchDownloadRequest = new BatchDownloadRequest(ConversationId: iterateRecordings.ConversationId, RecordingId: iterateRecordings.Id);
            batchRequest.Add(batchDownloadRequest);

            // Create the batch job with the request list
            BatchDownloadJobSubmission batchSubmission = new BatchDownloadJobSubmission(BatchDownloadRequestList: batchRequest);

            BatchDownloadJobSubmissionResult recordingBatchRequestId = new BatchDownloadJobSubmissionResult();
            RecordingApi recordingApi = new RecordingApi();
            recordingBatchRequestId = recordingApi.PostRecordingBatchrequests(batchSubmission);

            recordingStatus(recordingBatchRequestId);
        }

        /// <summary>
        /// Check status of generating url for downloading, if the result is still unavailble. The function will be called again until the result is available.
        /// </summary>
        /// <param name="recordingBatchRequestId"></param>
        /// <returns></returns>
        private void recordingStatus(BatchDownloadJobSubmissionResult recordingBatchRequestId)
        {
            BatchDownloadJobStatusResult getRecordingBatchRequestData = new BatchDownloadJobStatusResult();
            RecordingApi recordingApi = new RecordingApi();
            getRecordingBatchRequestData = recordingApi.GetRecordingBatchrequest(recordingBatchRequestId.Id);

            if (getRecordingBatchRequestData.ExpectedResultCount == getRecordingBatchRequestData.ResultCount)
            {
                // Pass the getRecordingBatchRequestData to getExtension function
                getExtension(getRecordingBatchRequestData);
            }
            else
            {
                Thread.Sleep(5000);
                recordingStatus(recordingBatchRequestId);
            }
        }

        /// <summary>
        /// Get extension of every recordings
        /// </summary>
        /// <param name="getRecordingBatchRequestData"></param>
        /// <returns></returns>
        private void getExtension(BatchDownloadJobStatusResult getRecordingBatchRequestData)
        {
            // Store the contentType to a variable that will be used later to determine the extension of recordings.
            string contentType = getRecordingBatchRequestData.Results[0].ContentType;
            // Split the text and gets the extension that will be used for the recording
            string ext = contentType.Split('/').Last();

            createDirectory(ext, getRecordingBatchRequestData);
        }

        /// <summary>
        /// Generate directory for recordings that will be downloaded
        /// </summary>
        /// <param name="ext"></param>
        /// <param name="getRecordingBatchRequestData"></param>
        /// <returns></returns>
        private void createDirectory(string ext, BatchDownloadJobStatusResult getRecordingBatchRequestData)
        {
            Console.WriteLine("Processing please wait...");

            string conversationId = getRecordingBatchRequestData.Results[0].ConversationId;
            string recordingId = getRecordingBatchRequestData.Results[0].RecordingId;
            string url = getRecordingBatchRequestData.Results[0].ResultUrl;

            string path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = path.Substring(0, path.LastIndexOf("bin"));
            System.IO.Directory.CreateDirectory(path + "\\Recordings\\" + conversationId + "_" + recordingId);

            downloadRecording(url, ext, path + "\\Recordings\\" + conversationId + "_" + recordingId);
        }

        /// <summary>
        /// Download recordings
        /// </summary>
        /// <param name="url"></param>
        /// <param name="ext"></param>
        /// <param name="targetDirectory"></param>
        /// <returns></returns>
        private void downloadRecording(string url, string ext, string targetDirectory)
        {
            // string downloadFile = conversationId + '_' + recordingId + '.' + ext;
            string filename = targetDirectory.Substring(targetDirectory.LastIndexOf('\\') + 1, 73);

            using (WebClient wc = new WebClient())
                wc.DownloadFile(url, targetDirectory + "\\" + filename + "." + ext);
        }

        /// <summary>
        /// Request client credentials token from PureCloud
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <returns></returns>
        private string GetToken(string clientId, string clientSecret)
        {
            var accessTokenInfo = Configuration.Default.ApiClient.PostToken(clientId, clientSecret);
            string token = accessTokenInfo.AccessToken;

            return token;
        }

        public void RunStep()
        {
            DownloadRecordings();
            Globals.LogPerformance("DownloadRecordings");
        }

    }
}
```
