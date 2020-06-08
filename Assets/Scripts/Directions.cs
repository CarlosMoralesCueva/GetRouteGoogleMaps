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
