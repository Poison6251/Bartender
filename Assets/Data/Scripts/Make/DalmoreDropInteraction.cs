using Item;
using UnityEngine;
using UnityEngine.Events;
using static Item.ItemData;

public class DalmoreDropInteraction : MonoBehaviour
{
    public UnityEvent dropEvent;
    public Ingredients Ingredient;

    private void OnTriggerEnter(Collider other)
    {
        var interaction = other.gameObject.GetComponent<IItemDataInteractable>();
        if(interaction != null)
        {
            Drop(interaction);
        }
    }
    private void Awake()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
    public void Drop(IItemDataInteractable interaction)
    {
        dropEvent?.Invoke();
    }
    public void PoolingRelease()
    {
        gameObject.SetActive(false);
    }
}
