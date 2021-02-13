using UnityEngine;
using UnityEngine.EventSystems;
 
public class buttonStats : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler {
	public bool selectUnitOnClick = true;
	
	private GameObject stats;
	private CharacterManager manager;
	
	void Start(){
		//Encontramos los botones del canvas
		stats = transform.Find("Stats").gameObject;
		stats.SetActive(false);
		
		if(selectUnitOnClick)
			manager = GameObject.FindObjectOfType<CharacterManager>();
	}
	
    public void OnPointerEnter (PointerEventData eventData) {
        stats.SetActive(true);
    }
 
    public void OnPointerExit (PointerEventData eventData) {
        stats.SetActive(false);
    }
	
	public void OnPointerUp (PointerEventData eventData) {
        stats.SetActive(false);
    }
	public void OnPointerDown (PointerEventData eventData) {
        stats.SetActive(false);
		if(selectUnitOnClick)
			manager.selectUnit(int.Parse(transform.name));
    }
}
