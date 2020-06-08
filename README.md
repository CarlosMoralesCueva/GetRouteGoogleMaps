## Programa desarrollado en Unity que muestra una ruta entre dos ubicaciones en un teléfono Android


### Pasos previos 

* Instalar la versión 2019.3.14f1 de Unity
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
