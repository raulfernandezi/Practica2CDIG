using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Controlador : MonoBehaviour
{
    [SerializeField] Transform[] elementos;
    [SerializeField] List<Receta> recetas;
    [SerializeField] TextMeshProUGUI textoReceta;
    [SerializeField] List<TipoElementoGameObject> prefabs;

    private int numUtensilios;
    private int numIngredientes;
    private Dictionary<TipoElemento, GameObject> elementosActivos;

    [Header("Configuración Animación")]
    [SerializeField] GameObject prefabPanAnimado;
    private GameObject instanciaPanAnimado;
    private bool recetaCompleta = false;
    private int indicePasoAnimacion = 0;
    private float velocidadAnimacion = 3.0f; 

    [Header("Configuración Información")]
    private bool mostrarInfo = false;

    [cite_start] 
    private TipoElemento[] ordenPasos = {
        TipoElemento.BANDEJA,
        TipoElemento.HUEVO,
        TipoElemento.SARTEN,
        [cite_start]TipoElemento.AZUCAR,
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

        foreach (Transform elemento in elementos)
        {
            DefaultObserverEventHandler e = elemento.GetComponent<DefaultObserverEventHandler>();
            e.OnTargetFound.AddListener(() => DeteccionElemento(elemento));
            e.OnTargetLost.AddListener(() => DeteccionElementoPerdido(elemento));
        }

        if (prefabPanAnimado != null)
        {
            instanciaPanAnimado = Instantiate(prefabPanAnimado);
            instanciaPanAnimado.SetActive(false);
        }
        ActualizarTextoReceta();
    }

    void Update()
    {
        [cite_start] 
        if (Input.GetKeyDown(KeyCode.I))
        {
            mostrarInfo = !mostrarInfo;
            GestionarTextosInformativos();
        }

        [cite_start]
        if (Input.GetKey(KeyCode.A) && recetaCompleta)
        {
            MoverPanEnBucle();
        }
        else if (instanciaPanAnimado != null)
        {
            instanciaPanAnimado.SetActive(false);
        }
    }

    private void GestionarTextosInformativos()
    {
        foreach (var par in elementosActivos)
        {
            TextMeshPro texto = par.Value.GetComponentInChildren<TextMeshPro>();
            if (texto != null)
            {
                texto.enabled = mostrarInfo;
                texto.text = par.Key.ToString();
            }
        }
    }

    private void MoverPanEnBucle()
    {
        if (instanciaPanAnimado == null) return;

        TipoElemento destinoActual = ordenPasos[indicePasoAnimacion];

        if (elementosActivos.ContainsKey(destinoActual))
        {
            instanciaPanAnimado.SetActive(true);
            Transform targetPos = elementosActivos[destinoActual].transform;

            instanciaPanAnimado.transform.position = Vector3.MoveTowards(
                instanciaPanAnimado.transform.position,
                targetPos.position,
                velocidadAnimacion * Time.deltaTime
            );

            if (Vector3.Distance(instanciaPanAnimado.transform.position, targetPos.position) < 0.05f)
            {
                indicePasoAnimacion = (indicePasoAnimacion + 1) % ordenPasos.Length;
            }
        }
        else
        {
            Debug.LogWarning("Animación pausada: Falta el activador de " + destinoActual);

            instanciaPanAnimado.SetActive(false);
        }
    }

    public void DeteccionElemento(Transform target)
    {
        Elemento componente = target.GetComponent<Elemento>();
        if (componente == null) return;

        TipoElemento tipoElemento = componente.GetTipoElemento();
        if (elementosActivos.ContainsKey(tipoElemento)) return;

        GameObject objetoVisual = target.GetChild(0).gameObject;
        elementosActivos.Add(tipoElemento, objetoVisual);

        if (Elemento.EsUtensilio(tipoElemento)) numUtensilios++;
        if (Elemento.EsIngredienteBasico(tipoElemento)) numIngredientes++;

        ActualizarReceta_Ver2();
        ActualizarTextoReceta();
    }

    public void DeteccionElementoPerdido(Transform target)
    {
        TipoElemento tipoElemento = target.GetComponent<Elemento>().GetTipoElemento();
        if (elementosActivos.ContainsKey(tipoElemento))
        {
            elementosActivos.Remove(tipoElemento);
            if (Elemento.EsUtensilio(tipoElemento)) numUtensilios--;
            if (Elemento.EsIngredienteBasico(tipoElemento)) numIngredientes--;

            ActualizarReceta_Ver2();
            ActualizarTextoReceta();
        }
    }


    private void ActualizarTextoReceta()
    {
        [cite_start]
        bool tieneIngredientes = numIngredientes >= 6;
        bool tieneUtensilios = numUtensilios >= 3;

        if (tieneIngredientes && tieneUtensilios)
        {
            textoReceta.text = "Receta completa";
            textoReceta.color = Color.green;
            recetaCompleta = true;
        }
        else if (!tieneIngredientes && !tieneUtensilios)
        {
            textoReceta.text = "Faltan elementos";
            textoReceta.color = Color.red;
            recetaCompleta = false;
        }
        else if (!tieneUtensilios)
        {
            textoReceta.text = "Faltan utensilios";
            textoReceta.color = Color.red;
            recetaCompleta = false;
        }
        else
        {
            textoReceta.text = "Faltan ingredientes";
            textoReceta.color = Color.red;
            recetaCompleta = false;
        }
    }
}