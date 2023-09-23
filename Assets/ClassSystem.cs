using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassSystem : MonoBehaviour
{
    [Header("In-Game Sectors")]
    public GameObject[] slots;
    public GameObject[] equipments;
    public GameObject[] perks;

    [Header("Pre Defined Resources")]
    public GameObject[] weaponList;
    public GameObject[] EquipmentList;
    public GameObject[] perkList;
    [Tooltip("Weapon that is in use will be sent here.")]
    public GameObject Holder;

    public bool m_slot1;
    public bool m_slot2;
    public bool m_slot3;
    public float currentSlot = 0;


    void Update()
    {
        Scroll();
        Slot();
    }

    void Scroll()
    {
        float y = Input.mouseScrollDelta.y;
        
        if (y > 0) // Scroll up
        {
            currentSlot++;
            if (currentSlot > 3)
                currentSlot = 1;
        }
        else if (y < 0) // Scroll down
        {
            currentSlot--;
            if (currentSlot < 1)
                currentSlot = 3;
        }
    }

    void Start()
    {
        // Instantiate all weapons under their respective slots
        for (int i = 0; i < weaponList.Length && i < slots.Length; i++)
        {
            ManageWeapon(i, i);
        }

        m_slot3 = true; // default slot is 3
        UpdateSlotParenting();  // Ensure the default slot is set as a child of the Holder
    }

    void Slot()
    {
        m_slot1 = (currentSlot == 1);
        m_slot2 = (currentSlot == 2);
        m_slot3 = (currentSlot == 3);

        UpdateSlotParenting();
    }

    void UpdateSlotParenting()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i + 1 == currentSlot)
            {
                slots[i].transform.SetParent(Holder.transform, true); // Move selected slot to Holder
                slots[i].SetActive(true); // Activate the selected slot
            }
            else
            {
                slots[i].transform.SetParent(this.transform, true); // Move other slots back to the main class object
                slots[i].SetActive(false); // Deactivate other slots
            }
        }
    }


    void ManageWeapon(int slotIndex, int weaponIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length) return;

        // Destroy existing weapon in the slot
        if (slots[slotIndex].transform.childCount > 0)
        {
            Destroy(slots[slotIndex].transform.GetChild(0).gameObject);
        }

        // Instantiate new weapon under the slot
        GameObject newWeapon = Instantiate(weaponList[weaponIndex], slots[slotIndex].transform.position, slots[slotIndex].transform.rotation);
        newWeapon.transform.localScale = new Vector3(newWeapon.transform.localScale.x / 4f, newWeapon.transform.localScale.y / 4f, newWeapon.transform.localScale.z / 4f);
        newWeapon.transform.SetParent(slots[slotIndex].transform);

    }
    
}
