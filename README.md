## Programa desarrollado en Unity que muestra una ruta entre dos ubicaciones en un teléfono Android


### Pasos previos 

* Instalar la versión 2019.3.14f1 de Unity
* Si se desea hacer debug en el teléfono Android descargar de Play Store Unity Remote 5

### Creación del programa

1. Iniciar Unity Hub y crear un nuevo programa 3D llamado SaveDataLocally
2. Ir a File, Build Settings ...
3. Seleccionar Android, dar clic en Switch Platform
4. En la pestaña Game cambiar la resolución de pestaña a 1280x720 Portrait
5. Ingresar un cuadro de texto dando clic derecho en la escena, UI, Text y configurar el cuadro de la siguiente manera:
* Nombre: NombreCuadro, Pos X: 0, Pos Y: 70, Width: 160, Height: 30, Font Size: 24, Alignment: Center 
6. Ingresar un campo para ingreso de datos dando clic derecho en la escena, UI, InputField y configurarlo de la siguiente manera: 
* Nombre: Nombre, Pos X: 0, Pos Y: 0, Width: 160, Height: 30
7. Ingresar un botón para guardar la información dando clic derecho en la escena, UI, Button y configurarlo de la siguiente manera: 
* Nombre: Guardar, Pos X: 0, Pos Y: -150, Width: 160, Height: 30
8. Ingresar un botón para guardar la información dando clic derecho en la escena, UI, Button y configurarlo de la siguiente manera: 
* Nombre: Resetear, Pos X: 0, Pos Y: -150, Width: 160, Height: 30
9. En la parte inferior, en Assets dar clic derecho y crear la carpeta Scripts
10. En esta carpeta crear un script de C# con el nombre de SaveDataLocally
11. Copiar el siguiente código en el script
```c#
using UnityEngine;
using UnityEngine.UI;

public class SaveDataLocally : MonoBehaviour
{
	public InputField nombre;
	public Text nombreCuadro;

	public void SaveButton()
	{
		PlayerPrefs.SetString("nombre", nombre.text);
		nombreCuadro.text = PlayerPrefs.GetString("nombre");
	}

    private void Start()
    {
		nombreCuadro.text = PlayerPrefs.GetString("nombre") ?? "";
    }

	public void Reset()
    {
		PlayerPrefs.DeleteKey("nombre");
		nombreCuadro.text = "";
    }
}
```
12. Una vez copiado el código añadir el script SaveDataLocally a la camara principal
13. Asociar la función SaveButton a la funcionalidad OnClick() del botón Guardar
14. Asociar la función Reset a la funcionalidad OnClick() del botón Resetear
22. Ingresar a File/Build Settings, dar clic  en "Add Open Scenes" y Build
23. Probar
