using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Vuforia;


public class Controlador : MonoBehaviour
{
    [SerializeField] Transform[] imageTarguets;
    [SerializeField] List<Receta> recetas;
    [SerializeField] TextMeshProUGUI textoReceta;
    [SerializeField] List<TipoElementoGameObject> prefabs;

    private int numUtensilios;
    private int numIngredientes;
    private Dictionary<TipoElemento, GameObject> elementosActivos;
    private Dictionary<TipoElemento, GameObject> prefabsInstanciados;

    bool mostrarNombre;


    [Header("Configuración Animación")]
    [SerializeField] GameObject prefabPanAnimado;
    private GameObject instanciaPanAnimado;
    private bool recetaCompleta;
    private int indicePasoAnimacion;
    private float velocidadAnimacion;
    private const float ALTURA_ANIMACION = 1.5f;

    private TipoElemento[] ordenPasos = {
        TipoElemento.BANDEJA,
        TipoElemento.HUEVO,
        TipoElemento.SARTEN,
        TipoElemento.AZUCAR,
        TipoElemento.PLATO
    };

    [Serializable]
    public struct TipoElementoGameObject
    {
        public TipoElemento tipoElemento;
        public GameObject gameObject;
    }

    void Start()
    {
        elementosActivos = new Dictionary<TipoElemento, GameObject>();
        prefabsInstanciados = new Dictionary<TipoElemento, GameObject>();
        numIngredientes = 0;
        numUtensilios = 0;
        mostrarNombre = false;
        recetaCompleta = false;
        indicePasoAnimacion = 0;
        velocidadAnimacion = 2.0f;

        foreach (Transform imageTarguet in imageTarguets)
        {
            DefaultObserverEventHandler observer = imageTarguet.GetComponent<DefaultObserverEventHandler>();
            observer.OnTargetFound.AddListener(() => DeteccionElemento(imageTarguet));
            observer.OnTargetLost.AddListener(() => DeteccionElementoPerdido(imageTarguet));
        }
        InstanciarPrefabs();
        ActualizarTextoReceta();

        if (prefabPanAnimado != null)
        {
            instanciaPanAnimado = Instantiate(prefabPanAnimado);
            instanciaPanAnimado.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            mostrarNombre = !mostrarNombre;
            Debug.Log(mostrarNombre);
        }

        if (Input.GetKey(KeyCode.A) && recetaCompleta)
        {
            MoverPanEnBucle();
        }
        else if (instanciaPanAnimado != null)
        {
            instanciaPanAnimado.SetActive(false);
        }
    }

    void OnGUI()
    {
        if (!mostrarNombre) return;
        foreach (Transform targuet in imageTarguets)
        {
            ObserverBehaviour observer = targuet.GetComponent<ObserverBehaviour>();
            TipoElemento tipoElemento = targuet.GetComponent<Elemento>().GetTipoElemento();
            bool trackeado = observer.TargetStatus.Status == Status.TRACKED;

            if (!trackeado) continue;


            Vector3 puntoMundo = targuet.transform.position + Vector3.down * 4f;

            Vector3 posPantalla = Camera.main.WorldToScreenPoint(puntoMundo);
            posPantalla.y = Screen.height - posPantalla.y;
            GUIStyle estilo = new GUIStyle(GUI.skin.textField);
            estilo.fontSize = 50;
            GUI.Label(new Rect(posPantalla.x, posPantalla.y, 300, 60), tipoElemento.ToString(), estilo);
        }
    }

    private void InstanciarPrefabs()
    {
        foreach (TipoElementoGameObject prefab in prefabs)
        {
            //TODO a veces no se pone bien el padre
            Transform padre = BuscarPadre(prefab.tipoElemento);
            GameObject nuevoObjeto = Instantiate(prefab.gameObject, padre,
                            false);

            nuevoObjeto.SetActive(false);
            prefabsInstanciados.Add(prefab.tipoElemento, nuevoObjeto);
        }
    }

    private Transform BuscarPadre(TipoElemento tipo)
    {
        TipoElemento tipoPadre = recetas.Find(r => r.resultado.Equals(tipo)).elementoPrincipal;
        Transform padre = imageTarguets.ToList().Find(i => i.GetComponent<Elemento>().GetTipoElemento().Equals(tipoPadre));
        return padre;
    }

    public void DeteccionElemento(Transform targuet)
    {
        TipoElemento tipoElemento = targuet.GetComponent<Elemento>().GetTipoElemento();

        if (elementosActivos.ContainsKey(tipoElemento)) return;

        GameObject nuevoObjeto = targuet.GetChild(0).gameObject;

        nuevoObjeto.SetActive(true);

        elementosActivos.Add(tipoElemento, nuevoObjeto);

        if (Elemento.EsUtensilio(tipoElemento)) numUtensilios++;
        if (Elemento.EsIngredienteBasico(tipoElemento)) numIngredientes++;

        recetaCompleta = numUtensilios >= Elemento.NUM_UTENSILIOS && numIngredientes >= Elemento.NUM_INGREDIENTES;
        if (recetaCompleta) { 
            instanciaPanAnimado.transform.position = elementosActivos[TipoElemento.BANDEJA].gameObject.transform.position;
            instanciaPanAnimado.transform.position += new Vector3(0, ALTURA_ANIMACION, 0);
        }
        ActualizarRecetaAumento();
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

            recetaCompleta = false;
            ActualizarRecetaDisminucion();
            ActualizarTextoReceta();
        }
    }

    private void ActualizarRecetaAumento()
    {
        foreach (Receta receta in recetas)
        {
            TipoElemento elementoPrincipal = receta.elementoPrincipal;

            bool estaPrincipal = elementosActivos.ContainsKey(elementoPrincipal);

            if (estaPrincipal)
            {

                List<TipoElemento> ingredientesReceta = receta.ingredientes.ToList();
                int ingredientesPresentes = 0;

                foreach (TipoElemento elemento in ingredientesReceta)
                {
                    if (elementosActivos.ContainsKey(elemento))
                    {
                        ingredientesPresentes++;
                    }
                    if (ingredientesReceta.Count == ingredientesPresentes) { break; }
                }

                if (ingredientesReceta.Count == ingredientesPresentes)
                {
                    TipoElemento resultado = receta.resultado;


                    if (!elementosActivos.ContainsKey(resultado))
                    {

                        GameObject nuevoElemento = prefabsInstanciados[resultado].gameObject;

                        elementosActivos.Add(resultado, nuevoElemento);


                        nuevoElemento.SetActive(true);

                        if (elementoPrincipal == TipoElemento.SARTEN || elementoPrincipal == TipoElemento.AZUCAR)
                        {
                            elementosActivos[elementoPrincipal].gameObject.SetActive(false);

                        }
                        foreach (TipoElemento elemento in ingredientesReceta)
                        {
                            elementosActivos[elemento].gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }

    private void ActualizarRecetaDisminucion()
    {
        foreach (Receta receta in recetas)
        {
            TipoElemento elementoResultado = receta.resultado;

            bool estaResultado = elementosActivos.ContainsKey(elementoResultado);

            if (estaResultado)
            {

                List<TipoElemento> ingredientesReceta = receta.ingredientes.ToList();
                int ingredientesPresentes = 0;
                TipoElemento elementoPrincipal = receta.elementoPrincipal;
                foreach (TipoElemento elemento in ingredientesReceta)
                {
                    if (elementosActivos.ContainsKey(elemento))
                    {
                        ingredientesPresentes++;
                    }
                    if (ingredientesReceta.Count == ingredientesPresentes) { break; }
                }

                if (ingredientesReceta.Count > ingredientesPresentes || !elementosActivos.ContainsKey(elementoPrincipal))
                {
                    elementosActivos[elementoResultado].gameObject.SetActive(false);

                    elementosActivos.Remove(elementoResultado);


                    if (elementosActivos.ContainsKey(elementoPrincipal))
                        elementosActivos[elementoPrincipal]?.gameObject.SetActive(true);


                    foreach (TipoElemento elemento in ingredientesReceta)
                    {
                        if (elementosActivos.ContainsKey(elemento))
                            elementosActivos[elemento]?.gameObject.SetActive(true);
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

    private void MoverPanEnBucle()
    {
        if (instanciaPanAnimado == null) return;
        instanciaPanAnimado.SetActive(true);

        TipoElemento destinoActual = ordenPasos[indicePasoAnimacion];

        if (elementosActivos.ContainsKey(destinoActual))
        {
            Transform targetPos = elementosActivos[destinoActual].transform.parent;

            Vector3 targuet = targetPos.position;
            targuet += new Vector3(0, ALTURA_ANIMACION, 0);
            instanciaPanAnimado.transform.position = Vector3.MoveTowards(
                instanciaPanAnimado.transform.position,
                targuet,
                velocidadAnimacion * Time.deltaTime
            );

            if (Vector3.Distance(instanciaPanAnimado.transform.position, targuet) < 0.1f)
            {
                indicePasoAnimacion = (indicePasoAnimacion + 1) % ordenPasos.Length;
            }
        }
    }
}
