using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public class Controlador : MonoBehaviour
{
    [SerializeField] Transform[] elementos;
    [SerializeField] List<Receta> recetas;
    [SerializeField] TextMeshProUGUI textoReceta;
    [SerializeField] List<TipoElementoGameObject> prefabs;

    private int numUtensilios;
    private int numIngredientes;
    private Dictionary<TipoElemento, GameObject> elementosActivos;

    [Serializable]
    public struct TipoElementoGameObject
    {
        public TipoElemento tipoElemento;
        public GameObject gameObject;
    }

    void Start()
    {
        elementosActivos = new Dictionary<TipoElemento, GameObject>();
        numIngredientes = 0;
        numUtensilios = 0;
        foreach (Transform elemento in elementos) {
            DefaultObserverEventHandler e = elemento.GetComponent<DefaultObserverEventHandler>();
            e.OnTargetFound.AddListener(() => DeteccionElemento(elemento));
            e.OnTargetLost.AddListener(() => DeteccionElementoPerdido(elemento));
        }
        ActualizarTextoReceta();
    }

    public void DeteccionElemento(Transform targuet)
    {
        TipoElemento tipoElemento = targuet.GetComponent<Elemento>().GetTipoElemento();

        if (elementosActivos.ContainsKey(tipoElemento)) return;

        GameObject nuevoObjeto = targuet.GetChild(0).gameObject;

        elementosActivos.Add(tipoElemento, nuevoObjeto);

        if (Elemento.EsUtensilio(tipoElemento)) numUtensilios++;
        if (Elemento.EsIngredienteBasico(tipoElemento)) numIngredientes++;

        ActualizarReceta_Ver2();
        ActualizarTextoReceta();
    }

    public void DeteccionElementoPerdido(Transform targuet)
    {
        TipoElemento tipoElemento = targuet.GetComponent<Elemento>().GetTipoElemento();
        if (elementosActivos.ContainsKey(tipoElemento))
        {
            elementosActivos.Remove(tipoElemento);

            if (Elemento.EsUtensilio(tipoElemento)) numUtensilios--;
            if (Elemento.EsIngredienteBasico(tipoElemento)) numIngredientes--;

            ActualizarReceta_Ver2();
            ActualizarTextoReceta();
        }
    }

    private void ActualizarReceta_Ver2()
    {
        foreach (Receta receta in recetas)
        {
            TipoElemento elementoPrincipal = receta.elementoPrincipal;

            bool estaPrincipal = elementosActivos.ContainsKey(elementoPrincipal);

            if (estaPrincipal)
            {
                // Copiamos la lista de ingredientes necesarios. 
                // Usar Remove() es m�s seguro que un contador por si hay objetos del mismo tipo duplicados.
                List<TipoElemento> ingredientesReceta = receta.ingredientes.ToList();
                int ingredientesPresentes = 0;

                foreach (TipoElemento elemento in ingredientesReceta)
                {
                    // Si el elemento detectado es uno de los que nos falta, lo tachamos de la lista
                    if (elementosActivos.ContainsKey(elemento))
                    {
                        ingredientesPresentes++;
                    }
                    if (ingredientesReceta.Count == ingredientesPresentes) { break; }
                }

                // 2. Si est�n todos los ingredientes presentes, creamos el resultado
                if (ingredientesReceta.Count == ingredientesPresentes)
                {
                    TipoElemento resultado = receta.resultado;

                    // Comprobamos que no est� presente en escena

                    if (!elementosActivos.ContainsKey(resultado))
                    {
                        
                        GameObject prefabAResultado = prefabs.Find((p) => p.tipoElemento.Equals(resultado)).gameObject;
                        Transform padrePrincipal = elementosActivos[elementoPrincipal].gameObject.transform.parent;


                        GameObject nuevoObjeto = Instantiate(prefabAResultado, Vector3.zero,
                            Quaternion.identity);



                        nuevoObjeto.transform.SetParent(padrePrincipal);

                        nuevoObjeto.transform.position = Vector3.zero;

                        elementosActivos.Add(resultado,nuevoObjeto);

                        elementosActivos[elementoPrincipal].gameObject.SetActive(false);
                        foreach (TipoElemento elemento in ingredientesReceta)
                        {
                            elementosActivos[elemento].gameObject.SetActive(false);
                        }

                        //TODO hay que ver como se eliminan los prefabs de ingredientes;

                        Debug.Log("�Receta completada!: " + resultado.ToString());

                        // Rompemos el bucle principal para no fabricar m�ltiples recetas a la vez
                        break;
                    }
                    else
                    {
                        Debug.LogWarning("No hay prefab asignado en el diccionario para: " + resultado);
                    }
                }
            }
        }
    }


    private void ActualizarTextoReceta()
    {
        if (numUtensilios < Elemento.NUM_UTENSILIOS && numIngredientes < Elemento.NUM_INGREDIENTES)
        {
            textoReceta.text = "Faltan elementos";
            textoReceta.color = Color.red;
        }
        else if (numUtensilios < Elemento.NUM_UTENSILIOS)
        {
            textoReceta.text = "Faltan utensilios";
            textoReceta.color = Color.red;
        }
        else if (numIngredientes < Elemento.NUM_INGREDIENTES)
        {
            textoReceta.text = "Faltan ingredientes";
            textoReceta.color = Color.red;
        }
        else
        {
            textoReceta.text = "Receta completa";
            textoReceta.color = Color.green;
        }
    }
}
