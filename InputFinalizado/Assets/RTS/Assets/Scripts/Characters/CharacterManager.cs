using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;



[System.Serializable]
public class Troop
{
    public GameObject deployableTroops;
    public int troopCost;
    public Sprite buttonImage;
    [HideInInspector]
    public GameObject button;


}
public class CharacterManager : MonoBehaviour
{


    public int starGold;
    public GUIStyle rectangleStyle;
    public Texture2D cursorTexture;
    public Texture2D cursorTexture1;
    public bool highlightSelectedButton;
    public Color buttonHightlight;
    public GameObject button;
    public float bombLoadingSpeed;
    public float BombRange;
    public GameObject bombExplosion;



    [Space(10)]
    public List<Troop> troops;
    //cariables no visibles
    public static Vector3 clickedPos;
    public static int gold;
    public static GameObject target;

    [Space(8)]
    private Vector2 mouseLastPos;
    private Vector2 mouseDownPos;
    private bool visible;
    private bool isDown;
    private GameObject[] knights;

    private int selectedUnit;
    [SerializeField]
    private GameObject goldText;
    private GameObject goldWarning;
    private GameObject addedGoldText;
    private GameObject characterList;
    private GameObject characterParent;
    private GameObject selectButton;
    [SerializeField]
    private GameObject bombLoadingBar;
    private GameObject bombButton;
    private float bombProgress;
    private bool isplacingBomb;
    private GameObject bombRange;
    public KeyCode deselectKey;
    public static bool selectionMode;
    public void addCharacterButtons()
    {

        for (int i = 0; i < troops.Count; i++)
        {
            GameObject newButton = Instantiate(button);
            RectTransform rectTransform = newButton.GetComponent<RectTransform>();
            rectTransform.SetParent(characterList.transform, false);
            newButton.GetComponent<Outline>().effectColor = buttonHightlight;
            newButton.GetComponent<Image>().sprite = troops[i].buttonImage;
            newButton.transform.name = "" + i;
            newButton.GetComponentInChildren<Text>().text = "Price: " + troops[i].troopCost +
                "\n Damage: " + troops[i].deployableTroops.GetComponentInChildren<Character>().damage +
                "\n Lives: " + troops[i].deployableTroops.GetComponentInChildren<Character>().lives;

            //this is the new button
            troops[i].button = newButton;

        }



    }
    private void Awake()
    {


        //======>Renderizar el canvas
    
        //Encontrar los Objectos
        characterParent = new GameObject("Characters");
        selectButton = GameObject.Find("Character selection button");
        target = GameObject.Find("target");
     

        //Conseguir los elementos del canvas del Au
        characterList = GameObject.Find("Character buttons");
        //goldText = GameObject.Find("gold Text");
        addedGoldText = GameObject.Find("added gold text");
        goldWarning = GameObject.Find("gold warning");

        //encontrar los objetos de las bombas
       // bombLoadingBar = GameObject.Find("Loading Bar");
        bombButton = GameObject.Find("Bomb button");
        bombRange = GameObject.Find("Bomb range");

    }

    private void Start()
    {
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        target.SetActive(true);
        //FIJAR EL CURSOR Y ADCIONAR LOS BOTONES DE LOS  PERSONAJES

        gold = starGold;
        addedGoldText.SetActive(true);
        goldWarning.SetActive(false);


        //FIJAR EL TIEMPO DE ADICION DE ORO (5minutos)

        InvokeRepeating("AddGold", 1.0f, 5.0f);


        bombRange.SetActive(false);
        isplacingBomb = false;
        addCharacterButtons();
    }

    private void Update()
    {
        if (bombProgress < 1)
        {

            //la bomba esta desactiva =>Cambia Color Rojo y desactiva el boton
            bombProgress += Time.deltaTime * bombLoadingSpeed;
            bombLoadingBar.GetComponent<Image>().color = Color.red;
            bombButton.GetComponent<Button>().enabled = false;
        }
        else
        {
            //La bomba esta activada =>Cambia a color Azul y activala
            bombProgress = 1f;
            bombLoadingBar.GetComponent<Image>().color = Color.blue;
            bombButton.GetComponent<Button>().enabled = true;
        }
       
        bombLoadingBar.GetComponent<Image>().fillAmount = bombProgress;

        //===============================ORO(AU)===========================================

        goldText.GetComponent<Text>().text = "" + gold;

        //===============================INPUT===========================================
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit);


        if (Input.GetMouseButtonDown(0))
        {

            //==========NO TIENESSUFICIENTE ORO=============
            if (gold < troops[selectedUnit].troopCost && !EventSystem.current.IsPointerOverGameObject())
            {
                StartCoroutine(GoldWarning());
            }
            //============Si EL MOUSE RAY COLLISIONO CON ALGO  =============
            if (hit.collider != null)
            {
                //============Si EL MOUSE RAY COLLISIONO GROUND && TIENE DINERO  && NO SEAS MODO SELECCION DE MULTIPLES PERSONJES   =============            
                if (hit.collider.gameObject.CompareTag("Battle ground") && !selectionMode && !isplacingBomb && !EventSystem.current.IsPointerOverGameObject() && gold >= troops[selectedUnit].troopCost)
                {
                  CreateUnit(hit);
                }
                //============Si EL MOUSE RAY COLLISIONO GROUND &&  MODO instanciar bomba   =============            
                if (hit.collider.gameObject.CompareTag("Battle ground") && isplacingBomb && !EventSystem.current.IsPointerOverGameObject())
                {
                  //=========Bomb=============
                    Instantiate(bombExplosion, hit.point, Quaternion.identity);
                    //Reiniciar el counter Progres
                    bombProgress = 0;
                    isplacingBomb = false;
                    bombRange.SetActive(false);
                    //=========ENCONTRAR ENEMIGOS DENTRO DEL RANGO=============
                    GameObject[] insideRange = GameObject.FindGameObjectsWithTag("Enemy");
                    foreach (GameObject enemy in insideRange)
                    {
                        if (enemy != null && Vector3.Distance(enemy.transform.position, hit.point) <= BombRange / 2)
                        {

                            enemy.GetComponent<Character>().lives = 0;
                        }
                    }
                }
                //============Si EL MOUSE RAY COLLISIONO GROUND &&  MODO instanciar bomba &&  y despues quiso instanciar en la ui OJO despues   =============            
                else if (hit.collider.gameObject.CompareTag("Battle ground") && isplacingBomb && EventSystem.current.IsPointerOverGameObject())
                {
                    isplacingBomb = false;
                    bombRange.SetActive(false);
                }
            }
        }
        //============Si EL MOUSE RAY COLLISIONO GROUND &&  MODO instanciar bomba &&  NO APRETO EL BUTTON =>LUZ   =============            
        if (hit.collider != null && isplacingBomb && !EventSystem.current.IsPointerOverGameObject())
        {
            //====OBJETO DE RANGE BOMB SE ACTIVA
            bombRange.transform.position = new Vector3(hit.point.x, 75, hit.point.z);
            bombRange.GetComponent<Light>().spotAngle = BombRange;
            bombRange.SetActive(true);
        }

      


        //==============Si apretas  Space activas el modo SELECCION DE MULTIPLES PERSONJES
        if (Input.GetMouseButtonDown(0) && selectionMode && !isplacingBomb)
        {
            mouseDownPos = Input.mousePosition;
            isDown = true;
            visible = true;// <== Rect  Transform
        }
        if (isDown)
        {
            mouseLastPos = Input.mousePosition;
            if (Input.GetMouseButtonUp(0))
            {
                isDown = false;
                visible = false;
            }
        }

        //==================Apretas Space==============
        if (Input.GetKeyDown(KeyCode.Space))
        {
            selectCharacters();
        }
      //==========encuentra los personajes
        knights = GameObject.FindGameObjectsWithTag("Knight");

        //==============desmarcar a todos los personajes seleccionados

        if (Input.GetKeyDown(deselectKey))
        {
            foreach (GameObject knight in knights)
            {
                if (knight != null)
                {
                    Character character = knight.GetComponent<Character>();
                    character.selected = false;
                    character.selectedObject.SetActive(false);
                }
            }
        }

        StartCoroutine(CheckGold());

    }
    IEnumerator GoldWarning()
    {
        if (!goldWarning.activeSelf)
        {
            goldWarning.SetActive(true);
        }
        yield return new WaitForSeconds(2f);
        goldWarning.SetActive(false);
    }
    IEnumerator CheckGold()
    {
        for (int i = 0; i < troops.Count; i++)
        {
            if (troops[i].button != null)
            {
                if (troops[i].troopCost <= gold)
                {


                    troops[i].button.gameObject.GetComponent<Image>().color = Color.white;

                }
                else
                {

                    troops[i].button.gameObject.GetComponent<Image>().color = Color.black;


                }
            }
        }

        yield return new WaitForSeconds(3f);
    }

    //=======================Crear unidad el selectedUnit lo Obtenemos de la classe ... 
    public void CreateUnit(RaycastHit hit)
    {   GameObject newTroop = Instantiate(troops[selectedUnit].deployableTroops,
        hit.point, troops[selectedUnit].deployableTroops.transform.rotation) as GameObject;
        newTroop.transform.parent = characterParent.transform;
        gold -= troops[selectedUnit].troopCost;
    }

    private void OnGUI()
    {
        //======================ONGUI metodo com update 
        //==================Apretado Space Fijamos la imagen Rango de Referencia

        if (visible)
        {
            Vector2 origin;
            origin.x = Mathf.Min(mouseDownPos.x, mouseLastPos.x);

            //==========GUI================

            origin.y = Mathf.Max(mouseDownPos.y, mouseLastPos.y);
            origin.y = Screen.height - origin.y;

            //tamaño de la malla que marca los objetos

            Vector2 size = mouseDownPos - mouseLastPos;
            size.x = Mathf.Abs(size.x);
            size.y = Mathf.Abs(size.y);

            Rect rect = new Rect(origin.x, origin.y, size.x, size.y);
            GUI.Box(rect, "", rectangleStyle);


            //=========================== tood slos personajes selecionados
            foreach (GameObject knight in knights)
            {
                if (knights != null)
                {
                    Vector3 pos = Camera.main.WorldToScreenPoint(knight.transform.position);
                    pos.y = Screen.height - pos.y;
                    if (rect.Contains(pos))
                    {
                        Character character = knight.GetComponent<Character>();
                        character.selected = true;
                        character.selectedObject.SetActive(true);
                    }
                }
            }
        }
    }

    public void selectUnit(int unit)
    {

        if (highlightSelectedButton)
        {
            for (int i = 0; i < troops.Count; i++)
            {
                if (troops[i].button != null)
                {
                    troops[i].button.GetComponent<Outline>().enabled = false;

                }

            }
            troops[selectedUnit].button.GetComponent<Outline>().enabled = true;
        }
        selectedUnit = unit;

    }

    public void selectCharacters()
    {
        selectionMode = !selectionMode;
        if (selectionMode)
        {
            //===============setCursoColor SELECCION
            selectButton.GetComponent<Image>().color = Color.red;
            Cursor.SetCursor(cursorTexture1, Vector2.zero, CursorMode.Auto);

        }
        else
        {

            selectButton.GetComponent<Image>().color = Color.white;
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
            foreach (GameObject knight in knights)
            {
                if (knight != null)
                {
                    Character character = knight.GetComponent<Character>();
                    character.selected = false;
                    character.selectedObject.SetActive(false);
                }
            }

        }
    }

    void AddGold()
    {
        gold += 100;
        StartCoroutine(AddedGoldText());
    }
    IEnumerator AddedGoldText()
    {
        addedGoldText.SetActive(true);
        yield return new WaitForSeconds(0.7f);
        addedGoldText.SetActive(false);
    }
    public void placeBomb()
    {

        isplacingBomb = true;
    }



}

